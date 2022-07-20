using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sistema_de_Asesoramiento_legal_en_el_área_del_trabajo
{
    class Extractor
    {
        string path;
        int fila = 1;
        Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
        Workbook wb;
        Worksheet ws;

        public Extractor(string path)
        {
            this.path = path;
            wb = excel.Workbooks.Open(path);
            ws = wb.Worksheets[1];
        }
        public List<List<Regla>> extraerReglas()
        {
            List<List<Regla>> listaDeListasDeReglas = new List<List<Regla>>();
            while(!String.IsNullOrEmpty(valorCelda(fila+1, 1)))
            {
                fila++;
                listaDeListasDeReglas.Add(obtenerListaDeReglas());
            }
            wb.Close();
            excel.Quit();
            return listaDeListasDeReglas;
        }

        private List<Regla> obtenerListaDeReglas()
        {
            List<Regla> listaDeReglas = new List<Regla>();
            List<string> listaPremisas = new List<string>();
            List<string> listaDeNivelesDeVerdad = new List<string>();
            while(!String.IsNullOrEmpty(valorCelda(fila,1)))
            {
                listaPremisas.Clear();
                string nombre = valorCelda(fila, 1);
                string pregunta = valorCelda(fila, 2);
                string explicacion = valorCelda(fila, 3);
                string indicacion = valorCelda(fila, 5);
                string premisas = valorCelda(fila, 4);
                string NivelesDeVerdadNecesarios = valorCelda(fila, 6);
                if (String.IsNullOrEmpty(premisas))
                {
                    premisas = "NaN";
                    NivelesDeVerdadNecesarios = "NaN";
                }
                listaPremisas = separarString(premisas);
                listaDeNivelesDeVerdad = separarString(NivelesDeVerdadNecesarios);
                listaDeReglas.Add(new Regla(nombre, pregunta, explicacion, listaPremisas, indicacion, listaDeNivelesDeVerdad));
                fila++;
            }
            return listaDeReglas;
        }
        private string valorCelda(int i, int j)
        {
            return ((string)(ws.Cells[i, j] as Microsoft.Office.Interop.Excel.Range).Value);
        }

        private List<string> separarString(string premisas)
        {
            List<string> listaPremisas = new List<string>();
            string[] premisasSeparadas = premisas.Split(",");
            foreach (string premisa in premisasSeparadas)
            {
                listaPremisas.Add(premisa);
            }
            return listaPremisas;
        }
    }
}
