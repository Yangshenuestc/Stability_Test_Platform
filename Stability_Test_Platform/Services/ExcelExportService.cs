using ClosedXML.Excel;
using Stability_Test_Platform.DataProcessing;
using System.IO;
using System.Linq;

namespace Stability_Test_Platform.Services
{
    public class ExcelExportService
    {
        /// <summary>
        /// 保存原始 IV 数据 (电压步长为表头，时间为行首)
        /// </summary>
        public void AppendIvDataToExcel(string filePath, string deviceId, double timeHours, double[] voltage, double[] current)
        {
            bool fileExists = File.Exists(filePath);

            using (var workbook = fileExists ? new XLWorkbook(filePath) : new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.FirstOrDefault() ?? workbook.Worksheets.Add("IV_Data");

                // 如果是新文件，初始化第一行的表头
                if (!fileExists || worksheet.LastRowUsed() == null)
                {
                    worksheet.Cell(1, 1).Value = deviceId; // A1: 1-1

                    // B1, C1, D1... 填入电压步长值
                    for (int i = 0; i < voltage.Length; i++)
                    {
                        worksheet.Cell(1, i + 2).Value = voltage[i];
                    }
                    worksheet.Row(1).Style.Font.Bold = true;
                    worksheet.SheetView.FreezeRows(1); // 冻结首行
                }

                // 找到最后一行空行写入新数据
                int nextRow = (worksheet.LastRowUsed()?.RowNumber() ?? 1) + 1;

                worksheet.Cell(nextRow, 1).Value = timeHours; // A2, A3... 填入时间

                // B2, C2, D2... 填入对应的电流数据 (或者电流密度 J，取决于你传入的 current 数组)
                for (int i = 0; i < current.Length; i++)
                {
                    worksheet.Cell(nextRow, i + 2).Value = current[i];
                }

                workbook.SaveAs(filePath);
            }
        }

        /// <summary>
        /// 保存 Stability Result 综合参数数据 (仅保留核心参数列)
        /// </summary>
        public void AppendResultDataToExcel(string filePath, string deviceId, PvMeasurementData data)
        {
            bool fileExists = File.Exists(filePath);

            using (var workbook = fileExists ? new XLWorkbook(filePath) : new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.FirstOrDefault() ?? workbook.Worksheets.Add("Result");

                // 如果是新文件，初始化表头 (严格从 A1 开始)
                if (!fileExists || worksheet.LastRowUsed() == null)
                {
                    worksheet.Cell(1, 1).Value = "Time(h)";                 // A1
                    worksheet.Cell(1, 2).Value = "Jsc(mA/cm2)";             // B1
                    worksheet.Cell(1, 3).Value = "Voc(V)";                  // C1
                    worksheet.Cell(1, 4).Value = "FF";                      // D1
                    worksheet.Cell(1, 5).Value = "Pmax";                    // E1
                    worksheet.Cell(1, 6).Value = "Vmpp";                    // F1
                    worksheet.Cell(1, 7).Value = "Rse (Ohm/cm2)";           // G1
                    worksheet.Cell(1, 8).Value = "Rsh (Ohm/cm2)";           // H1
                    worksheet.Cell(1, 9).Value = "Direction";               // I1
                    worksheet.Cell(1, 10).Value = "Delay(s)";               // J1
                    worksheet.Cell(1, 11).Value = "Tem(℃)";                // K1

                    // 表头加粗并冻结首行
                    worksheet.Row(1).Style.Font.Bold = true;
                    worksheet.SheetView.FreezeRows(1);
                }

                // 获取下一行空白行
                int nextRow = (worksheet.LastRowUsed()?.RowNumber() ?? 1) + 1;

                // 填入数据
                worksheet.Cell(nextRow, 1).Value = data.TimeHours;                               // A列: 时间
                worksheet.Cell(nextRow, 2).Value = System.Math.Round(data.Jsc, 4);               // B列: Jsc
                worksheet.Cell(nextRow, 3).Value = System.Math.Round(data.Voc, 4);               // C列: Voc
                worksheet.Cell(nextRow, 4).Value = System.Math.Round(data.FF, 4);                // D列: FF
                worksheet.Cell(nextRow, 5).Value = System.Math.Round(data.Pmax, 4);              // E列: Pmax
                worksheet.Cell(nextRow, 6).Value = System.Math.Round(data.Vmpp, 4);              // F列: Vmpp
                worksheet.Cell(nextRow, 7).Value = System.Math.Round(data.Rseries, 2);           // G列: Rseries
                worksheet.Cell(nextRow, 8).Value = System.Math.Round(data.Rshunt, 2);            // H列: Rshunt

                // I列: 将 bool 类型的 SweepDirection 转为对应字符串写入
                worksheet.Cell(nextRow, 9).Value = data.SweepDirection ? "Forward" : "Reverse";

                worksheet.Cell(nextRow, 10).Value = data.DelaySeconds;                           // J列: 延迟
                worksheet.Cell(nextRow, 11).Value = System.Math.Round(data.Temperature, 1);      // K列: 温度

                workbook.SaveAs(filePath);
            }
        }
    }
}