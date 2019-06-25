using CsvHelper;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ReplaceWordsInTextFile.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {

        public bool CanExecute { get; set; }

        private string filePath;
        public string FilePath
        {
            get { return filePath; }
            set { SetProperty(ref filePath, value); }
        }

        private string mappingFilePath;
        public string MappingFilePath
        {
            get { return mappingFilePath; }
            set { SetProperty(ref mappingFilePath, value); }
        }

        private Dictionary<string, string> MappingTable { get; set; } = new Dictionary<string, string>();

        public DelegateCommand OkCommand { get; private set; }
        public DelegateCommand CancelCommand { get; private set; }

        public DelegateCommand ChooseFileCommand { get; private set; }
        public DelegateCommand ChooseMappingFileCommand { get; private set; }

        public MainWindowViewModel()
        {
            OkCommand = new DelegateCommand(() => { CanExecute = true; ReplaceRoiNames(); });
            CancelCommand = new DelegateCommand(() => { CanExecute = false; });

            ChooseFileCommand = new DelegateCommand(ChooseFile);
            ChooseMappingFileCommand = new DelegateCommand(ChooseMappingFile);
        }

        private void ReplaceRoiNames()
        {

            using (var reader = new StreamReader(FilePath))
            {
                var fileContent = new StringBuilder(reader.ReadToEnd());

                foreach (KeyValuePair<string, string> item in MappingTable)
                {
                    var oldWord = item.Key;
                    var newWord = item.Value;

                    fileContent.Replace("\"" + oldWord + "\"", "\"" + newWord + "\"");
                }

                var directoryName = Path.GetDirectoryName(FilePath);
                var fileName = Path.GetFileName(FilePath);
                var newFileName = "New_" + fileName;
                var newFilePath = Path.Combine(directoryName, newFileName);

                using (var writer = new StreamWriter(newFilePath))
                {
                    writer.Write(fileContent);
                }
            }
        }

        private void ChooseFile()
        {
            var dialog = new OpenFileDialog();
            dialog.Title = "ファイルを開く";
            dialog.Filter = "全てのファイル(*.*)|*.*";
            if (dialog.ShowDialog() == true)
            {
                FilePath = dialog.FileName;
            }
            else
            {
                FilePath = string.Empty;
            }
        }

        private void ChooseMappingFile()
        {
            var dialog = new OpenFileDialog();
            dialog.Title = "Mappingファイルを開く";
            dialog.Filter = "CSVファイル(*.csv)|*.csv";
            if (dialog.ShowDialog() == true)
            {
                MappingFilePath = dialog.FileName;

                using (var reader = new StreamReader(MappingFilePath))
                using (var csv = new CsvReader(reader))
                {
                    MappingTable.Clear();
                    var records = csv.GetRecords<dynamic>();
                    foreach (var r in records)
                    {
                        MappingTable[r.Old] = r.New;
                    }
                }
            }
            else
            {
                MappingFilePath = string.Empty;
            }
        }
    }
}
