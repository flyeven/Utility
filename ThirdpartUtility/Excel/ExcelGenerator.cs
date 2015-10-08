using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.ThirdpartUtility.Excel
{
    /// <summary>
    /// Excel生成辅助工具
    /// </summary>
    public class ExcelGenerator
    {
        /// <summary>
        /// DataTable导出到Excel(使用模板)
        /// </summary>
        /// <param name="source"></param>
        /// <param name="saveFileName"></param>
        /// <param name="templateFileName"></param>
        public static void DataTableToExcelByTemplate(DataTable source, string saveFileName, string templateFileName)
        {
            //检查
            if (!File.Exists(templateFileName))
                throw new FileNotFoundException(templateFileName);

            //创建工作簿
            FileStream tf = File.OpenRead(templateFileName);
            IWorkbook workbook = new XSSFWorkbook(tf);

            //获取Sheet
            //ISheet sheet = workbook.GetSheet("Sheet1");
            ISheet sheet = workbook.GetSheetAt(0);

            //表头使用模板样式
            int index = 1;      //行下标,从第二行开始插入数据
            for (int r = 0; r < source.Rows.Count; r++)
            {
                //拷贝上一行的数据和样式到本行
                //目的是为可复用上一行的样式，这样模板中只需要设置一行的样式即可
                if (index - 1 > 0)
                {
                    IRow preRow = sheet.GetRow(index - 1);
                    if (preRow != null)
                        preRow = preRow.CopyRowTo(index);
                }

                //获取行,null则创建
                IRow row = sheet.GetRow(index) ?? sheet.CreateRow(index);

                #region 赋值
                for (int c = 0; c < source.Columns.Count; c++)
                {
                    var value = source.Rows[r][c];

                    //获取单元格，null则创建
                    //ICell cell = row.CreateCell(i);
                    ICell cell = row.GetCell(c, MissingCellPolicy.CREATE_NULL_AS_BLANK);
                    switch (Type.GetTypeCode(value.GetType()))
                    {
                        case TypeCode.DateTime:
                            {
                                cell.SetCellValue(Convert.ToDateTime(value));
                            }
                            break;
                        case TypeCode.Boolean:
                            {
                                cell.SetCellValue(Convert.ToBoolean(value));
                            }
                            break;
                        case TypeCode.Decimal:
                        case TypeCode.Double:
                            {
                                cell.SetCellValue(Convert.ToDouble(value));
                            }
                            break;
                        case TypeCode.DBNull:
                            {
                                cell.SetCellValue(string.Empty);
                            }
                            break;
                        default:
                            cell.SetCellValue(value.ToString());
                            break;
                    }
                }
                #endregion

                index++;
            }


            //创建excel文件
            FileStream sw = File.Create(saveFileName);
            workbook.Write(sw);
            sw.Close();

            //释放模板文件
            tf.Close();
        }

        /// <summary>
        /// DataTable导出到Excel【暂无用】
        /// </summary>
        /// <param name="source"></param>
        /// <param name="saveFileName"></param>
        public static void DataTableToExcel(DataTable source, string saveFileName)
        {
            //创建工作簿
            IWorkbook workbook = new XSSFWorkbook();

            //创建Sheet
            ISheet sheet = workbook.CreateSheet("Sheet1");
            //删除sheet
            //workbook.RemoveSheetAt(0);

            //获取Sheet
            //ISheet sheet = workbook.GetSheet("Sheet1");
            //ISheet sheet = workbook.GetSheetAt(0);

            //创建表头
            int r = 0;      //行下标
            IRow rowTitle = sheet.CreateRow(r);
            rowTitle.HeightInPoints = 20;      //设置表头高为20点

            for (int i = 0; i < source.Columns.Count; i++)
            {
                //设置列宽
                sheet.SetColumnWidth(i, 5120);  //20*256

                //赋值
                string title = source.Columns[i].ColumnName;
                ICell cell = rowTitle.CreateCell(i);
                cell.SetCellValue(title);

                //设置样式
                ICellStyle cellStyle = workbook.CreateCellStyle();
                cellStyle.Alignment = HorizontalAlignment.Center;
                cellStyle.VerticalAlignment = VerticalAlignment.Center;
                cellStyle.FillBackgroundColor = HSSFColor.LightOrange.Index;
                cellStyle.FillForegroundColor = HSSFColor.LightOrange.Index;
                cellStyle.FillPattern = FillPattern.SolidForeground;

                cell.CellStyle = cellStyle;
            }

            //创建表单
            r++;
            for (; r < source.Rows.Count; r++)
            {
                IRow row = sheet.CreateRow(r);
                for (int i = 0; i < source.Columns.Count; i++)
                {
                    var value = source.Rows[r][i];
                    ICell cell = row.CreateCell(i);

                    switch (Type.GetTypeCode(value.GetType()))
                    {
                        //datetime 格式特殊处理
                        case TypeCode.DateTime:
                            {
                                cell.SetCellValue(Convert.ToDateTime(value));
                                //set date format
                                ICellStyle cellStyle = workbook.CreateCellStyle();
                                IDataFormat format = workbook.CreateDataFormat();
                                cellStyle.DataFormat = format.GetFormat("yyyy/mm/dd");
                                cell.CellStyle = cellStyle;
                            }
                            break;

                        default:
                            cell.SetCellValue(value.ToString());
                            break;
                    }
                }
            }


            //创建excel文件
            FileStream sw = File.Create(saveFileName);
            workbook.Write(sw);
            sw.Close();
        }
    }
}
