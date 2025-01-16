using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;

namespace ApiaryDataWeb.Controllers
{
    public class ExportDataController : Controller
    {

        private readonly AppDbContext _context;

        public ExportDataController(AppDbContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {
            return View();
        }

        //[HttpPost]
        //public async Task<IActionResult> ExportData([FromBody] ExportRequest request)
        //{
        //    DateTime? fromDate = request.Period switch
        //    {
        //        "24h" => DateTime.Now.AddDays(-1),
        //        "7d" => DateTime.Now.AddDays(-7),
        //        "30d" => DateTime.Now.AddDays(-30),
        //        "all" => null,
        //        _ => throw new ArgumentException("Neplatné období.")
        //    };

        //    var data = request.Table switch
        //    {
        //        "HiveMeasurements" => fromDate.HasValue
        //            ? (object)await _context.HiveMeasurement.Where(m => m.MeasurementDate >= fromDate).ToListAsync()
        //            : (object)await _context.HiveMeasurement.ToListAsync(),
        //        "ApiaryMeasurements" => fromDate.HasValue
        //            ? (object)await _context.ApiaryMeasurement.Where(m => m.MeasurementDate >= fromDate).ToListAsync()
        //            : (object)await _context.ApiaryMeasurement.ToListAsync(),
        //        _ => throw new ArgumentException("Neplatná tabulka.")
        //    };

        //    if (request.Format == "json")
        //    {
        //        // Serializace dat do JSON
        //        var json = System.Text.Json.JsonSerializer.Serialize(data);
        //        var bytes = System.Text.Encoding.UTF8.GetBytes(json);

        //        // Vrací soubor JSON
        //        return File(bytes, "application/json", $"{request.Table}_{DateTime.Now:yyyyMMddHHmmss}.json");
        //    }
        //    else if (request.Format == "csv")
        //    {
        //        // Serializace dat do CSV
        //        var csv = SerializeToCsv(data);
        //        var bytes = System.Text.Encoding.UTF8.GetBytes(csv);

        //        // Vrací soubor CSV
        //        return File(bytes, "text/csv", $"{request.Table}_{DateTime.Now:yyyyMMddHHmmss}.csv");
        //    }

        //    return BadRequest("Neplatný formát.");
        //}
        [HttpPost]
        public async Task<IActionResult> ExportData([FromBody] ExportRequest request)
        {
            DateTime? fromDate = request.Period switch
            {
                "24h" => DateTime.Now.AddDays(-1),
                "7d" => DateTime.Now.AddDays(-7),
                "30d" => DateTime.Now.AddDays(-30),
                "all" => null,
                _ => throw new ArgumentException("Neplatné období.")
            };

            var data = request.Table switch
            {
                "HiveMeasurements" => fromDate.HasValue
                    ? (object)await _context.HiveMeasurement.Where(m => m.MeasurementDate >= fromDate).ToListAsync()
                    : (object)await _context.HiveMeasurement.ToListAsync(),
                "ApiaryMeasurements" => fromDate.HasValue
                    ? (object)await _context.ApiaryMeasurement.Where(m => m.MeasurementDate >= fromDate).ToListAsync()
                    : (object)await _context.ApiaryMeasurement.ToListAsync(),
                _ => throw new ArgumentException("Neplatná tabulka.")
            };

            if (request.Format == "json")
            {
                // Serializace dat do JSON
                var json = System.Text.Json.JsonSerializer.Serialize(data);
                var bytes = System.Text.Encoding.UTF8.GetBytes(json);

                // Vrací soubor JSON
                return File(bytes, "application/json", $"{request.Table}_{DateTime.Now:yyyyMMddHHmmss}.json");
            }
            else if (request.Format == "csv")
            {
                // Serializace dat do CSV
                var csv = SerializeToCsv(data);
                var bytes = System.Text.Encoding.UTF8.GetBytes(csv);

                // Vrací soubor CSV
                return File(bytes, "text/csv", $"{request.Table}_{DateTime.Now:yyyyMMddHHmmss}.csv");
            }

            return BadRequest("Neplatný formát.");
        }




        [HttpPost]
        public async Task<IActionResult> ExportSelectedEntities([FromBody] ExportSelectedRequest request)
        {
            // Vytvoření streamu pro ZIP archiv
            using (var memoryStream = new MemoryStream())
            {
                using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    // Pro každou vybranou entitu vytvoříme odpovídající CSV soubor
                    foreach (var entity in request.Entities)
                    {
                        // Měníme způsob získávání dat z switch výrazu
                        IEnumerable<object> data= entity switch
                        {
                            "Hives" => await _context.Hives
                                        .Select(h => new { h.HiveNumber, h.Name, h.ApiaryNumber })
                                        .ToListAsync(),
                            "Apiary" => await _context.Apiaries
                                        .Select(a => new { a.ApiaryNumber, a.Name })
                                        .ToListAsync(),
                            "ApiaryMeasurement" => await _context.ApiaryMeasurement
                                        .Select(am => new { am.Id, am.ApiaryNumber, am.MeasurementDate, am.Humidity, am.Temperature, am.LightIntensity })  // Vybrané sloupce
                                        .ToListAsync(),
                            "HiveMeasurement" => await _context.HiveMeasurement
                                        .Select(hm => new { hm.Id, hm.HiveNumber, hm.MeasurementDate, hm.Temperature, hm.Humidity, hm.Weight })  // Vybrané sloupce
                                        .ToListAsync(),
                            "InspectionReport" => await _context.InspectionReport
                                        .Select(ir => new { ir.ReportId, ir.InspectionDate, ir.QueenPresent, ir.SignsOfDisease, ir.AdequateFood, ir.HiveClean, ir.BroodPatternGood })  // Vybrané sloupce
                                        .ToListAsync(),
                            _ => throw new ArgumentException($"Neznámá entita: {entity}")
                        };

                        if (data.Any())
                        {
                            var csv = SerializeToCsv(data);

                            // Přidání souboru do ZIP archivu
                            var zipEntry = zipArchive.CreateEntry($"{entity}_{DateTime.Now:yyyyMMddHHmmss}.csv");
                            using (var entryStream = zipEntry.Open())
                            using (var writer = new StreamWriter(entryStream))
                            {
                                await writer.WriteAsync(csv);
                            }
                        }
                    }
                }
                // Zapsání paměti streamu do souboru
                memoryStream.Seek(0, SeekOrigin.Begin);  // Ujistíme se, že začneme číst od začátku

                // Vracíme ZIP archiv jako soubor ke stažení
                return File(memoryStream.ToArray(), "application/zip", "selected_entities.zip");
            }
        }

        // Pomocná metoda pro převod dat na CSV
        private string SerializeToCsv(object data)
        {
            if (data is IEnumerable<object> list)
            {
                var sb = new StringBuilder();

                // Získání hlavičky (názvy sloupců)
                var props = list.First().GetType().GetProperties();
                sb.AppendLine(string.Join(",", props.Select(p => p.Name)));

                // Přidání dat
                foreach (var item in list)
                {
                    sb.AppendLine(string.Join(",", props.Select(p => p.GetValue(item)?.ToString() ?? "")));
                }

                return sb.ToString();
            }

            throw new InvalidOperationException("Data nejsou ve správném formátu pro CSV.");
        }

        /*/ Metoda pro získání dat na základě entity
        private async Task<List<object>> GetEntityData(string entity, string period)
        {
            /*DateTime? fromDate = period switch
            {
                "24h" => DateTime.Now.AddDays(-1),
                "7d" => DateTime.Now.AddDays(-7),
                "30d" => DateTime.Now.AddDays(-30),
                "all" => null,
                _ => throw new ArgumentException("Neplatné období.")
            };

            switch (entity)
            {
                case "Hives":
                    return await _context.Hives.ToListAsync();
                case "Apiary":
                    return await _context.Apiaries.ToListAsync();
                case "ApiaryMeasurement":
                    return await _context.ApiaryMeasurement.ToListAsync();
                case "HiveMeasurement":
                    return await _context.HiveMeasurement.ToListAsync();
                case "InspectionReport":
                    return await _context.InspectionReports.ToListAsync();
                default:
                    throw new ArgumentException($"Neznámá entita: {entity}");
            }
        }*/

        /*public async Task<List<T>> GetMeasurementsAsync<T>(string table, DateTime? fromDate) where T : class
        {
            return table switch
            {
                "HiveMeasurements" => (fromDate.HasValue
                    ? await _context.HiveMeasurement.Where(m => m.MeasurementDate >= fromDate).ToListAsync()
                    : await _context.HiveMeasurement.ToListAsync()) as List<T>,
                "ApiaryMeasurements" => (fromDate.HasValue
                    ? await _context.ApiaryMeasurement.Where(m => m.MeasurementDate >= fromDate).ToListAsync()
                    : await _context.ApiaryMeasurement.ToListAsync()) as List<T>,
                _ => throw new ArgumentException("Neplatná tabulka.")
            };
        }*/

        /*/// <summary>
        /// Export všech datových tabulek.
        /// </summary>
        /// <returns>JSON nebo XML soubor</returns>
        [HttpGet]
        public async Task<IActionResult> ExportAll()
        {
            var measurements = await _context.Measurements.ToListAsync();
            var otherTable = await _context.OtherEntities.ToListAsync(); // Příklad další tabulky

            // Vytvoříme strukturu objektu
            var allData = new
            {
                Measurements = measurements,
                OtherEntities = otherTable
            };

            return Json(allData); // Vrací kompletní data ve formátu JSON
        }*/
    }



    public class ExportRequest
    {
        public string Table { get; set; }
        public string Period { get; set; }
        public string Format { get; set; }
    }
    public class ExportSelectedRequest
    {
        public List<string> Entities { get; set; }
        public string Period { get; set; }
    }
}
