using DevExpress.XtraReports.UI;
using final_aerialview.Data;
using System.ServiceModel;

namespace final_aerialview.Services
{
    public class CustomReportStorageWebExtension : DevExpress.XtraReports.Web.Extensions.ReportStorageWebExtension
    {
        private readonly string _reportDirectory;
        private readonly DataAccess _dataAccess;

        const string FileExtension = ".repx";


        public CustomReportStorageWebExtension(string reportDirectory, DataAccess dataAccess)
        {
            _reportDirectory = reportDirectory;
            _dataAccess = dataAccess;

            if (!Directory.Exists(_reportDirectory))
            {
                Directory.CreateDirectory(_reportDirectory);
            }

        }

        private bool IsWithinReportsFolder(string url, string folder)
        {
            var rootDirectory = new DirectoryInfo(folder);
            var fileInfo = new FileInfo(Path.Combine(folder, url));

            return fileInfo.Directory.FullName.ToLower().StartsWith(rootDirectory.FullName.ToLower());
        }

        public override bool CanSetData(string url)
        {
            // Determine whether it is possible to store a report by a given URL.
            return true;
        }

        public override bool IsValidUrl(string url)
        {
            // Determine whether the URL passed to the current Report Storage is valid.
            return Path.GetFileName(url) == url;
        }


        public override byte[] GetData(string url)
        {
            try
            {
                // Construct the full file path
                var filePath = Path.Combine(_reportDirectory, url + FileExtension);

                // Check if the file exists
                if (File.Exists(filePath))
                {
                    // Read and return the file content
                    return File.ReadAllBytes(filePath);
                }
                else
                {
                    // If the file doesn't exist, throw a custom exception
                    throw new FileNotFoundException($"Report file not found: {filePath}");
                }
            }
            catch (FileNotFoundException ex)
            {
                // Handle file not found exception
                throw new FaultException(new FaultReason($"Could not find report '{url}'. {ex.Message}"), new FaultCode("Server"), "GetData");
            }
            catch (IOException ex)
            {
                // Handle IO exception
                throw new FaultException(new FaultReason($"Error reading report file '{url}'. {ex.Message}"), new FaultCode("Server"), "GetData");
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                throw new FaultException(new FaultReason($"An error occurred while getting report data. {ex.Message}"), new FaultCode("Server"), "GetData");
            }
        }


        public override Dictionary<string, string> GetUrls()
        {
            // Return a dictionary of existing report URLs and display names.
            return Directory.GetFiles(_reportDirectory, "*" + FileExtension)
                .Select(Path.GetFileNameWithoutExtension)
                .ToDictionary<string, string>(x => x);
        }

        public override void SetData(XtraReport report, string url)
        {
            try
            {
                var fullReportPath = Path.GetFullPath(Path.Combine(_reportDirectory, url + FileExtension));
                if (!fullReportPath.StartsWith(_reportDirectory + Path.DirectorySeparatorChar))
                {
                    throw new FaultException("Invalid report name.");
                }

                report.SaveLayoutToXml(fullReportPath);
                Console.WriteLine($"Saved report path: {fullReportPath}");
                //_dataAccess.UpdateReportPath(fullReportPath);
                _dataAccess.UpdateReportPath(url);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating full report path: {ex.Message}");
            }
        }

        public override string SetNewData(XtraReport report, string defaultUrl)
        {
            // Store the specified report using a new URL.
            SetData(report, defaultUrl);
            return defaultUrl;
        }

    }
}
