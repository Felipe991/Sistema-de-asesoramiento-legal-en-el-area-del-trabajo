using System;
using System.Collections.Generic;
using System.Text;

namespace Sistema_de_Asesoramiento_legal_en_el_área_del_trabajo
{
    class NodoRegla
    {
        public int nivel;
        public Regla regla;
        public List<NodoRegla> nodosAnteriores;
        public List<NodoRegla> nodosSiguientes;

        public NodoRegla()
        {
            this.nivel = new int();
            this.regla = new Regla();
            this.nodosAnteriores = new List<NodoRegla>();
            this.nodosSiguientes = new List<NodoRegla>();
        }

        public override string ToString()
        {
            return this.regla.ToString() +
                "\nNivel: " + this.nivel +
                "\nNodos Anteriores: "+ obtenerNombresNodosAnteriores() +
                "\nNodos Siguientes: "+ obtenerNombresNodosSiguientes();

        }

        private string obtenerNombresNodosSiguientes()
        {
            string nombreNodos = " ";
            foreach (NodoRegla nodo in nodosSiguientes)
            {
                nombreNodos += nodo.regla.nombre + " ";
            }
            return nombreNodos;
        }

        private string obtenerNombresNodosAnteriores()
        {
            string nombreNodos = " ";
            foreach(NodoRegla nodo in nodosAnteriores)
            {
                nombreNodos += nodo.regla.nombre+" ";
            }
            return nombreNodos;
        }
    }
}
