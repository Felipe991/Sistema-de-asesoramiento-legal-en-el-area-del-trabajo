using System;
using System.Collections.Generic;
using System.Text;

namespace Sistema_de_Asesoramiento_legal_en_el_área_del_trabajo
{
    class Resolucion
    {
        List<NodoRegla> conclusiones;

        public Resolucion(List<NodoRegla> conclusiones)
        {
            this.conclusiones = conclusiones;
        }

        public void darResolucion()
        {
            imprimir("**************RESOLUCION DEL SISTEMA**************\n");
            if (hayConclusiones())
            {
                List<List<string>> explicaciones = new List<List<string>>();
                generarCaminosLogicos(explicaciones);
                mostrarResoluciones(explicaciones);
            }
            else
            {
                imprimir("Su situación no recae dentro de ningun artículo del libro 2 del código del trabajo");
            }
            imprimir("\n\n**************RESOLUCION DEL SISTEMA**************");
        }

        private void mostrarResoluciones(List<List<string>> explicaciones)
        {
            int numeroConclusion = 0;
            foreach (List<string> explicacion in explicaciones)
            {
                imprimir("\n");
                mostrarArticulo(numeroConclusion);
                mostrarIndicacion(numeroConclusion);
                mostrarCaminoLogico(explicacion, numeroConclusion);
                numeroConclusion++;
            }
        }

        private void mostrarIndicacion(int numeroConclusion)
        {
            imprimir("Indicacion: " + conclusiones[numeroConclusion].regla.indicaciones);
        }

        private void mostrarArticulo(int numeroConclusion)
        {
            imprimir("Nombre de conclusion: " + conclusiones[numeroConclusion].regla.nombre);
        }

        private void mostrarCaminoLogico(List<string> explicacion, int numeroConclusion)
        {
            imprimir("Explicacion:");
            for (int indice = explicacion.Count - 1; indice >= 0; indice--)
            {
                imprimir(explicacion[indice]);
            }
            imprimir("Su situacion recae dentro del " + conclusiones[numeroConclusion].regla.nombre);
        }

        private bool hayConclusiones()
        {
            return (conclusiones.Count > 0);
        }

        private void generarCaminosLogicos(List<List<string>> explicaciones)
        {
            foreach (NodoRegla conclusion in conclusiones)
            {
                List<string> explicacion = new List<string>();
                agregarExplicacion(conclusion, explicacion);
                explicaciones.Add(explicacion);
            }
        }

        private void agregarExplicacion(NodoRegla conclusion, List<string> caminoLogico)
        {
            if (!String.IsNullOrEmpty(conclusion.regla.explicacion) && conclusion.nodosSiguientes.Count == 0)
            {
                caminoLogico.Add(conclusion.regla.explicacion);
            }
            foreach (NodoRegla nodoAnterior in conclusion.nodosAnteriores)
            {
                if (!String.IsNullOrEmpty(nodoAnterior.regla.explicacion))
                {
                    caminoLogico.Add(nodoAnterior.regla.explicacion);
                }
                if (esUltimoNodoAnterior(nodoAnterior, conclusion.nodosAnteriores))
                {
                    agregarExplicacion(nodoAnterior, caminoLogico);
                }
            }
        }

        private bool esUltimoNodoAnterior(NodoRegla nodoAnterior, List<NodoRegla> nodosAnteriores)
        {
            return (nodosAnteriores[nodosAnteriores.Count - 1] == nodoAnterior);
        }

        private void imprimir(string cadena)
        {
            Console.WriteLine(cadena);
        }
    }
}
