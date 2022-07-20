using System;
using System.Collections.Generic;
using System.Text;

namespace Sistema_de_Asesoramiento_legal_en_el_área_del_trabajo
{
    public class Fuzzificador
    {
        public Fuzzificador()
        {

        }

        public string fuzzificar(float certeza)
        {
            string nivelCerteza = "";
            //Console.WriteLine("Certeza recibida: "+certeza);
            double[] valoresPertenencia = new double[5];
            obtenerValoresPertenencia(certeza, valoresPertenencia);
            nivelCerteza = obtenerNivelCerteza(valoresPertenencia);
            //imprimirValoresPertenencia(valoresPertenencia);
            return nivelCerteza;
        }

        private void imprimirValoresPertenencia(double[] valoresPertenencia)
        {
            string[] nivelesCerteza = { "Falso", "Probablemente falso", "Desconocido", "Probablemente verdadero", "Verdadero" };
            for (int i = 0; i < valoresPertenencia.Length; i++)
            {
                Console.WriteLine(nivelesCerteza[i] + "(" + valoresPertenencia[i] + ")");
            }
        }

        private string obtenerNivelCerteza(double[] valoresPertenencia)
        {
            string[] nivelesCerteza = { "Falso", "Probablemente falso", "Inseguro", "Probablemente verdadero", "Verdadero" };
            int indiceNivelMasPertenenciente = 0;
            Double pertenenciaMasAlta = 0;
            for (int indice = 4; indice >= 0; indice--)
            {
                if (valoresPertenencia[indice] >= pertenenciaMasAlta)
                {
                    indiceNivelMasPertenenciente = indice;
                    pertenenciaMasAlta = valoresPertenencia[indice];
                }
            }
            return nivelesCerteza[indiceNivelMasPertenenciente];
        }

        private void obtenerValoresPertenencia(float certeza, double[] valoresPertenencia)
        {
            valoresPertenencia[0] = funcTriangularIzq(certeza, 0, 0.25);
            valoresPertenencia[1] = funcTriangularMid(certeza, 0, 0.25, 0.5);
            valoresPertenencia[2] = funcTriangularMid(certeza, 0.4, 0.5, 0.6);
            valoresPertenencia[3] = funcTriangularMid(certeza, 0.5, 0.75, 1);
            valoresPertenencia[4] = funcTriangularDer(certeza, 0.75, 1);
        }

        private double funcTriangularIzq(double certeza, double limMed, double limDer)
        {
            double valorPertenencia = 0;
            if (certeza >= limMed && certeza <= limDer)
            {
                valorPertenencia = (limDer - certeza) / (limDer - limMed);
            }
            return valorPertenencia;
        }

        private double funcTriangularMid(double certeza, double limIzq, double limMed, double limDer)
        {
            double valorPertenencia = 0;
            if (certeza >= limIzq && certeza <= limMed)
            {
                valorPertenencia = (certeza - limIzq) / (limMed - limIzq);
            }
            else if (certeza >= limMed && certeza <= limDer)
            {
                valorPertenencia = (limDer - certeza) / (limDer - limMed);
            }
            return valorPertenencia;
        }

        private double funcTriangularDer(double certeza, double limIzq, double limMed)
        {
            double valorPertenencia = 0;
            if (certeza >= limIzq && certeza <= limMed)
            {
                valorPertenencia = (certeza - limIzq) / (limMed - limIzq);
            }
            return valorPertenencia;
        }
    }
}
