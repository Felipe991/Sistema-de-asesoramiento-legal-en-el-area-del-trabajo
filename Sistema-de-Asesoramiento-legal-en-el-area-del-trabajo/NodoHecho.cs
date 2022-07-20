using System;
using System.Collections.Generic;
using System.Text;

namespace Sistema_de_Asesoramiento_legal_en_el_área_del_trabajo
{
    class NodoHecho
    {
        public int nivel;
        public Hecho hecho;
        public bool recorrido;
        public List<NodoHecho> nodosAnteriores;
        public List<NodoHecho> nodosSiguientes;

        public NodoHecho()
        {
            this.nivel = new int();
            this.recorrido = false;
            hecho = new Hecho();
            this.nodosAnteriores = new List<NodoHecho>();
            this.nodosSiguientes = new List<NodoHecho>();
        }

        public override string ToString()
        {
            return this.hecho.ToString() +
                 "\nNivel: " + this.nivel +
                 "\n¿Se pasó por aca?: " +  (recorrido ? "si" : "no")+
                 "\nNodos Anteriores: " + obtenerNombresNodosAnteriores() +
                 "\nNodos Siguientes: " + obtenerNombresNodosSiguientes();

        }

        private string obtenerNombresNodosSiguientes()
        {
            string nombreNodos = " ";
            foreach (NodoHecho nodo in nodosSiguientes)
            {
                nombreNodos += nodo.hecho.nombre + " ";
            }
            return nombreNodos;
        }

        private string obtenerNombresNodosAnteriores()
        {
            string nombreNodos = " ";
            foreach (NodoHecho nodo in nodosAnteriores)
            {
                nombreNodos += nodo.hecho.nombre + " ";
            }
            return nombreNodos;
        }
    }
}
