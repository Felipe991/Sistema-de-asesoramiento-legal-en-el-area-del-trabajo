using System;
using Microsoft.Office.Interop.Excel;
using System.Collections.Generic;

namespace Sistema_de_Asesoramiento_legal_en_el_área_del_trabajo
{
    class Program
    {
        static void Main(string[] args)
        {
            Extractor extractor = new Extractor(AppDomain.CurrentDomain.BaseDirectory + "Reglas laborales - Codigo del trabajo - Libro2.xlsx");
            List<List<Regla>> listaDeListasDeReglas = new List<List<Regla>>();
            listaDeListasDeReglas.AddRange(extractor.extraerReglas());

            BaseDeReglas BDR = new BaseDeReglas(listaDeListasDeReglas);
            BaseDeHechos BDH = new BaseDeHechos(listaDeListasDeReglas);

            MotorDeInferencia motorDeInferencia = new MotorDeInferencia(BDR, BDH);
            motorDeInferencia.encadenarHaciaAdelante();

            Resolucion resolucion = new Resolucion(motorDeInferencia.conclusiones);
            resolucion.darResolucion();
        }
    }
}
