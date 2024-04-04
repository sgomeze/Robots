using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lbDataBase
{
    public class GetExcel
    {
        public DataSet DataExcel  { get; set; }
        public string ArchivoExcel { get; }

        public void FillData()
        {
            try
            {
                using (var stream = File.Open(ArchivoExcel, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                        {
                            UseColumnDataType = true,
                            FilterSheet = (tableReader, sheetIndex) => true,
                            ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                            {
                                EmptyColumnNamePrefix = "Columna",
                                UseHeaderRow = true
                            }
                        });
                        DataExcel =  result;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public GetExcel(string ArchivoExcel)
        {
            this.ArchivoExcel = ArchivoExcel;
            FillData();
        }
    }
}
