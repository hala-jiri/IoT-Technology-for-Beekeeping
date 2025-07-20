using BeeApp.Shared.Data;
using BeeApp.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;
using System.Text.Json;

namespace BeeApp.Web.Services
{
    public class BackupService
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public BackupService(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<Backup> CreateBackupAsync()
        {
            var backup = new Backup
            {
                Created = DateTime.Now
            };

            try
            {
                // folder for ZIP file
                var backupDir = Path.Combine(_env.WebRootPath, "backups");
                Directory.CreateDirectory(backupDir);

                var fileName = $"backup-{DateTime.Now:yyyyMMdd-HHmmss}.zip";
                var zipPath = Path.Combine(backupDir, fileName);

                // create zipfile
                using (var zipStream = new FileStream(zipPath, FileMode.Create))
                using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create))
                {
                    var now = DateTime.Now;

                    // load data
                    var hives = await _context.Hives
                        .Select(h => new {
                            h.HiveId,
                            h.Name,
                            h.ApiaryId
                        }).ToListAsync();

                    var measurements = await _context.HiveMeasurements
                        .Select(m => new {
                            m.HiveId,
                            m.MeasurementDate,
                            m.Weight,
                            m.Temperature
                        }).ToListAsync();

                    var inspections = await _context.InspectionReports
                        .Select(i => new {
                            i.HiveId,
                            i.InspectionDate,
                            i.QueenSeen,
                            i.Notes
                        }).ToListAsync();

                    var apiaries = await _context.Apiaries
                        .Select(a => new
                        {
                            a.ApiaryId,
                            a.Name,
                            a.Latitude,
                            a.Longitude,
                            a.ImageFileName,
                            Hives = a.Hives.Select(h => new
                            {
                                h.HiveId,
                                h.Name
                            }).ToList()
                        })
                        .ToListAsync();

                    // write to JSON files to ZIPu
                    AddJsonToZip(archive, "hives.json", hives);
                    AddJsonToZip(archive, "measurements.json", measurements);
                    AddJsonToZip(archive, "inspections.json", inspections);
                    AddJsonToZip(archive, "apiaries.json", apiaries);

                    // timerange
                    backup.DataFrom = measurements.Min(m => m.MeasurementDate);
                    backup.DataTo = measurements.Max(m => m.MeasurementDate);
                }

                // about ZIP
                var fileInfo = new FileInfo(zipPath);
                backup.FileName = fileName;
                backup.FileSize = fileInfo.Length;
                backup.Success = true;
                backup.Message = "Backup successful";
            }
            catch (Exception ex)
            {
                backup.Success = false;
                backup.Message = $"Error: {ex.Message}";
            }

            _context.Backups.Add(backup);
            await _context.SaveChangesAsync();

            return backup;
        }

        private void AddJsonToZip<T>(ZipArchive archive, string entryName, T data)
        {
            var entry = archive.CreateEntry(entryName);
            using var entryStream = entry.Open();

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles // needed to fix cycles of json objects
            };

            JsonSerializer.Serialize(new Utf8JsonWriter(entryStream, new JsonWriterOptions { Indented = true }), data, options);
        }
    }

}
