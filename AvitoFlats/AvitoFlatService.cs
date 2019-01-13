using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace AvitoFlats
{
    public class AvitoFlatService
    {
        private readonly AvitoFlatRepository _repository;
        private IEnumerable<Flat> _cachedItems;
        private Timer _timer;

        private string _url;
        private int _notificationPeriod;
        private EmailConfigs _emailConfig;
        private string _logPath;

        public AvitoFlatService(AvitoConfigs configs)
        {
            BuildConfigs(configs);
            _repository = new AvitoFlatRepository(_url);
            _cachedItems = new List<Flat>();
        }

        public void Start()
        {
            _timer = new Timer(SendNotificationAsync, null, 0, _notificationPeriod);
        }

        public async void SendNotificationAsync(object state)
        {
            var items = await _repository.GetCurrentAsync();
            
            var filteredItems = FiteredItems(items);
            
            await SendEmail(filteredItems);
            LogItemsToConsole(filteredItems);
            await LogItemsToFile(items);
            
            _cachedItems = new List<Flat>(items);
        }

        private IEnumerable<Flat> FiteredItems(IEnumerable<Flat> items)
        {
            var filteredItems = items.Except(_cachedItems, new FlatComparer()).ToList();
            
            foreach (var item in items)
            {
                if (filteredItems.Count >= 30)
                    break;
                
                if(!filteredItems.Contains(item, new FlatComparer()))
                    filteredItems.Add(item);
            }

            return filteredItems;
        }

        private void LogItemsToConsole(IEnumerable<Flat> items)
        {
            int i = 1;
            foreach (var item in items)
            {
                Console.WriteLine("===");
                Console.WriteLine($"{i}.  {item}");
                i++;
            }
        }

        private async Task LogItemsToFile(IEnumerable<Flat> items)
        {
            var date = DateTime.Now.ToString()
                .Replace(":", "-")
                .Replace("/", ".")
                .Replace("\\", ".");
            using (var outputFile = new StreamWriter(Path.Combine(_logPath, $"AvitoUpdates[{date}].txt"))) 
            {
                foreach (var line in items)
                {
                    await outputFile.WriteLineAsync(line.ToString());
                }
            }
        }

        private async Task SendEmail(IEnumerable<Flat> items)
        {
            if (!items.Any())
            {
                Console.WriteLine("No items for motification.");
                return;
            }
            
            try
            {
                var client = new SendGridClient(_emailConfig.SendGridKey);
                var message = BuildMessage(items);
                await client.SendEmailAsync(message);
                
                Console.WriteLine("Email has been sent!");
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Issue in email sending.");
                Console.WriteLine(e);
                Console.ForegroundColor = ConsoleColor.Black;
            }
        }

        private SendGridMessage BuildMessage(IEnumerable<Flat> items)
        {
            var from = new EmailAddress(_emailConfig.FromEmail, "Avito Update");
            var to = new EmailAddress(_emailConfig.ToEmails[0]);
            var subject = $"Avito updates {DateTime.Now}";
            
            var styles = "<style type=\"text/css\">.owner{ line-height: 18px; color: #858585; } .price{font-weight: 700;}</style>";
            var body = $"{styles} {string.Join("<br/>", items.Select(x => x.ToHtml()))}";
            
            var msg = MailHelper.CreateSingleEmail(from, to, subject, "", body);

            for (int i = 1; i < _emailConfig.ToEmails.Count; i++)
            {
                msg.AddTo(_emailConfig.ToEmails[i]);
            }

            return msg;
        }

        private void BuildConfigs(AvitoConfigs configs)
        {
            if(string.IsNullOrWhiteSpace(configs.BaseUrl))
                throw new ArgumentNullException("BaseUrl can not be null or empty.");
            
            if(configs.NotificationPeriod <= 0)
                throw new ArgumentNullException("NotificationPeriod should be greater then 0.");
            
            if(string.IsNullOrWhiteSpace(configs.EmailConfig.SendGridKey))
                throw new ArgumentNullException("SendGridKey can not be null or empty.");
            
            if(configs.EmailConfig.ToEmails.Count == 0)
                throw new ArgumentNullException("At list one ToEmail should be specified.");

            var queryParamsList = configs.QueryParams
                .Select(x => $"{x.Key}={x.Value}");

            _url = $"{configs.BaseUrl}{string.Join("&", queryParamsList)}";
            _notificationPeriod = configs.NotificationPeriod;
            _emailConfig = configs.EmailConfig;
            _logPath = configs.LogPath;
        }
    }
}