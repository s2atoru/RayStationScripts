using CsvHelper;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using Reactive.Bindings;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Reactive.Linq;
using System.Linq;
using System.Text;

namespace RoiNameReplacer.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {

        public bool CanExecute { get; set; }

        [Required(ErrorMessage ="Choose File!")]
        public ReactiveProperty<string> FilePath { get; }
        public ReadOnlyReactiveProperty<string> FilePathErrorMessage { get; }

        [Required(ErrorMessage = "Choose Mapping File!")]
        public ReactiveProperty<string> MappingFilePath { get; }
        public ReadOnlyReactiveProperty<string> MappingFilePathErrorMessage { get; }

        private Dictionary<string, string> MappingTable { get; set; } = new Dictionary<string, string>();

        public ReactiveCommand OkCommand { get; }

        public DelegateCommand CancelCommand { get; private set; }

        public DelegateCommand ChooseFileCommand { get; private set; }
        public DelegateCommand ChooseMappingFileCommand { get; private set; }

        public MainWindowViewModel()
        {

            FilePath = new ReactiveProperty<string>().SetValidateAttribute(() => FilePath);
            FilePathErrorMessage = FilePath
            .ObserveErrorChanged
            .Select(x => x?.Cast<string>()?.FirstOrDefault())
            .ToReadOnlyReactiveProperty();

            MappingFilePath = new ReactiveProperty<string>().SetValidateAttribute(() => MappingFilePath);
            MappingFilePathErrorMessage = MappingFilePath
            .ObserveErrorChanged
            .Select(x => x?.Cast<string>()?.FirstOrDefault())
            .ToReadOnlyReactiveProperty();

            //OkCommand = new DelegateCommand(() => { CanExecute = true; ReplaceRoiNames(); });
            OkCommand = new[]
            {
                FilePath.ObserveHasErrors,
                MappingFilePath.ObserveHasErrors
            }
            .CombineLatest(x => x.All(y => !y))
            .ToReactiveCommand();

            OkCommand.Subscribe(() => { CanExecute = true; ReplaceRoiNames(); });

            CancelCommand = new DelegateCommand(() => { CanExecute = false; });

            ChooseFileCommand = new DelegateCommand(ChooseFile);
            ChooseMappingFileCommand = new DelegateCommand(ChooseMappingFile);
        }

        private void ReplaceRoiNames()
        {

            using (var reader = new StreamReader(FilePath.Value))
            {
                var fileContent = new StringBuilder(reader.ReadToEnd());

                foreach (KeyValuePair<string, string> item in MappingTable)
                {
                    var oldWord = item.Key;
                    var newWord = item.Value;

                    // in double quotes
                    fileContent.Replace("\"" + oldWord + "\"", "\"" + newWord + "\"");
                    // in single quoutes
                    fileContent.Replace("'" + oldWord + "'", "'" + newWord + "'");
                    // in parentheses
                    fileContent.Replace("(" + oldWord + ")", "(" + newWord + ")");
                }

                var directoryName = Path.GetDirectoryName(FilePath.Value);
                var fileName = Path.GetFileName(FilePath.Value);
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
                FilePath.Value = dialog.FileName;
            }
            else
            {
                FilePath.Value = string.Empty;
            }
        }

        private void ChooseMappingFile()
        {
            var dialog = new OpenFileDialog();
            dialog.Title = "Mappingファイルを開く";
            dialog.Filter = "CSVファイル(*.csv)|*.csv";
            if (dialog.ShowDialog() == true)
            {
                MappingFilePath.Value = dialog.FileName;

                using (var reader = new StreamReader(MappingFilePath.Value))
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
                MappingFilePath.Value = string.Empty;
            }
        }
    }
}
