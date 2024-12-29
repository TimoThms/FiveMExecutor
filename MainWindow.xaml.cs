using System;
using System.Windows;
using System.Threading.Tasks;
using System.IO;

namespace FiveMExecutor
{
    public partial class MainWindow : Window
    {
        private bool isNoClipEnabled = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnDumper_Click(object sender, RoutedEventArgs e)
        {
            DumpServerFiles();
        }

        private void btnExecute_Click(object sender, RoutedEventArgs e)
        {
            ExecuteScript(txtScript.Text);
        }

        private void SpawnMoney_Click(object sender, RoutedEventArgs e)
        {
            string moneyScript = @"
                TriggerServerEvent('esx:giveInventoryItem', GetPlayerServerId(PlayerId()), 'item_money', 'money', 100000)
            ";
            ExecuteScript(moneyScript);
        }

        private void NoClip_Click(object sender, RoutedEventArgs e)
        {
            isNoClipEnabled = !isNoClipEnabled;
            string noClipScript = isNoClipEnabled ? 
                "SetEntityCollision(PlayerPedId(), false, false)" :
                "SetEntityCollision(PlayerPedId(), true, true)";
            ExecuteScript(noClipScript);
        }

        private async void DumpServerFiles()
        {
            try
            {
                string fivemPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FiveM");
                txtDumpOutput.Text = "Scanning FiveM directory...\n";
                
                await Task.Run(() =>
                {
                    foreach (string file in Directory.GetFiles(fivemPath, "*.*", SearchOption.AllDirectories))
                    {
                        Dispatcher.Invoke(() =>
                        {
                            txtDumpOutput.Text += $"Found: {file}\n";
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during dump: {ex.Message}");
            }
        }

        private readonly MemoryManager memoryManager;
        private readonly ProcessDetector processDetector;
        private readonly ScriptInjector scriptInjector;
        private readonly ServerFileParser serverFileParser;
        private readonly SecurityManager securityManager;

        public MainWindow()
        {
            InitializeComponent();
            
            memoryManager = new MemoryManager();
            processDetector = new ProcessDetector();
            scriptInjector = new ScriptInjector(memoryManager);
            serverFileParser = new ServerFileParser();
            securityManager = new SecurityManager();

            processDetector.FiveMProcessFound += OnFiveMProcessFound;
            StartProcessDetection();
        }

        private async void StartProcessDetection()
        {
            await processDetector.StartDetection();
        }

        private void OnFiveMProcessFound(object sender, Process process)
        {
            if (memoryManager.AttachToProcess("FiveM"))
            {
                Dispatcher.Invoke(() =>
                {
                    btnExecute.IsEnabled = true;
                    MessageBox.Show("Successfully connected to FiveM!");
                });
            }
        }

        private async void ExecuteScript(string script)
        {
            try
            {
                byte[] encryptedScript = securityManager.EncryptScript(script);
                bool success = scriptInjector.InjectScript(securityManager.DecryptScript(encryptedScript));
                
                if (success)
                {
                    MessageBox.Show("Script executed successfully!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Script execution failed: {ex.Message}");
            }
        }
    }
}
