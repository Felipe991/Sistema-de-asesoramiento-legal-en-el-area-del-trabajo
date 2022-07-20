using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sistema_de_Asesoramiento_legal_en_el_área_del_trabajo
{
    class ArbolDeReglas
    {
        public List<Regla> listaDeReglas;
        public List<NodoRegla> hojas;
        public NodoRegla raiz;
        private Regla reglaRaiz;
        private List<NodoRegla> nodosCreados;

        public ArbolDeReglas(List<Regla> listaDeReglas)
        {
            this.listaDeReglas = new List<Regla>();
            this.listaDeReglas.AddRange(listaDeReglas);
            this.hojas = new List<NodoRegla>();
            this.nodosCreados = new List<NodoRegla>();
            raiz = new NodoRegla();
            nodosCreados.Add(raiz);
            crearArbol();
            //imprimirArbol(raiz);
            //imprimirArbolGrafico();
        }

        private void encontrarReglaRaiz()
        {
            foreach (Regla regla in listaDeReglas)
            {
                foreach (string premisa in regla.premisas)
                {
                    if (premisa.Equals("NaN"))
                    {
                        reglaRaiz = regla;
                        raiz.regla = regla;
                        break;
                    }
                }
            }
        }

        private void crearArbol()
        {
            encontrarReglaRaiz();
            raiz.nivel = 0;
            List<NodoRegla> nodosNivelAnterior = new List<NodoRegla>();
            nodosNivelAnterior.Add(raiz);
            encontrarNodos(raiz, reglaRaiz, nodosNivelAnterior);
            asignarNiveles(raiz,0);
        }

        private void encontrarNodos(NodoRegla nodo,Regla reglaNodo, List<NodoRegla> nodosNivelAnterior)
        {
            nodo.nodosSiguientes.AddRange(encontrarNodosSiguientes(nodo, reglaNodo, nodosNivelAnterior));
        }

        private List<NodoRegla> encontrarNodosSiguientes(NodoRegla nodo, Regla reglaNodo, List<NodoRegla> nodosNivelAnterior)
        {
            List<NodoRegla> nodosSiguientesEncontrados = new List<NodoRegla>();
            List<Regla> reglasNodosSiguientes = new List<Regla>();
            List<NodoRegla> nodosExistentes = new List<NodoRegla>();
            //nodo.nodosAnteriores.AddRange(encontrarNodosAnteriores(nodo, reglaNodo, nodosNivelAnterior));

            foreach (Regla regla in listaDeReglas)
            {
                foreach (string premisa in regla.premisas)
                {
                    if (premisa.Equals(nodo.regla.nombre))
                    {
                        NodoRegla nodoSiguienteExistente = new NodoRegla();
                        if (regla.premisas.Count > 1)
                        {
                            nodoSiguienteExistente = buscarNodo(regla);
                            if (!nodoSiguienteExistente.regla.nombre.Equals(""))
                            {
                                nodosSiguientesEncontrados.Add(nodoSiguienteExistente);
                                nodosExistentes.Add(nodoSiguienteExistente);
                                reglasNodosSiguientes.Add(regla);
                                break;
                            }
                        }
                        NodoRegla nodoSgt = new NodoRegla();
                        nodoSgt.regla = regla;
                        nodoSgt.nodosAnteriores.Add(nodo);
                        nodosSiguientesEncontrados.Add(nodoSgt);
                        reglasNodosSiguientes.Add(regla);
                        nodosCreados.Add(nodoSgt);
                        nodo.nodosAnteriores.AddRange(encontrarNodosAnteriores(nodoSgt, reglaNodo, nodosNivelAnterior));
                    }
                }
            }
            int contador = 0;
            foreach (NodoRegla nodoSiguiente in nodosSiguientesEncontrados)
            {
                if (!nodosExistentes.Contains(nodoSiguiente))
                {
                    encontrarNodos(nodoSiguiente, reglasNodosSiguientes[contador], nodosSiguientesEncontrados);
                }
                contador++;
            }
            return nodosSiguientesEncontrados;
        }

        private NodoRegla buscarNodo(Regla regla)
        {
            NodoRegla nodoSiguienteExistente = new NodoRegla();
            foreach (NodoRegla nodoCreado in nodosCreados)
            {
                int contador = -1;
                if (mismoNombre(regla, nodoCreado))
                {
                    contador = 0;
                    foreach (NodoRegla nodoAnterior in nodoCreado.nodosAnteriores)
                    {
                        foreach (string premisa in regla.premisas)
                        {
                            if (nodoAnterior.regla.nombre.Equals(premisa))
                            {
                                contador++;
                            }
                        }
                    }
                }
                if (contador == nodoCreado.nodosAnteriores.Count)
                {
                    return nodoCreado;
                }
            }
            return nodoSiguienteExistente;
        }

        private bool mismoNombre(Regla regla, NodoRegla nodo)
        {
            return (nodo.regla.nombre.Equals(regla.nombre));
        }

        private List<NodoRegla> encontrarNodosAnteriores(NodoRegla nodo, Regla reglaNodo, List<NodoRegla> nodosNivelAnterior)
        {
            List<NodoRegla> nodosAnteriores = new List<NodoRegla>();
            foreach (NodoRegla nodoPadre in nodosNivelAnterior)
            {
                foreach (string premisa in reglaNodo.premisas)
                {
                    if (nodoPadre.regla.nombre.Equals(premisa))
                    {
                        nodosAnteriores.Add(nodoPadre);
                    }
                }
            }
            return nodosAnteriores;
        }

        private void asignarNiveles(NodoRegla nodo, int nivel)
        {
            nodo.nivel = nivel;
            if (String.IsNullOrEmpty(nodo.regla.indicaciones))
            {
                foreach (NodoRegla nodoSgt in nodo.nodosSiguientes)
                {
                    asignarNiveles(nodoSgt, (nivel+1));
                }
            }
            else
            {
                agregarNodoHoja(nodo);
            }
        }

        private void agregarNodoHoja(NodoRegla nodo)
        {
            hojas.Add(nodo);
        }

        private void imprimirArbol(NodoRegla nodo)
        {
            Console.WriteLine(nodo.ToString());
            if (String.IsNullOrEmpty(nodo.regla.indicaciones))
            {
                foreach (NodoRegla nodoSgt in nodo.nodosSiguientes)
                {
                    imprimirArbol(nodoSgt);
                }
            }
        }

        public void imprimirArbolGrafico()
        {
            Console.WriteLine("a");
            IEnumerable<NodoRegla> list;
            list = nodosCreados.OrderBy(u => u.nivel);
            foreach (NodoRegla nodo in list)
            {
                string nombre = nodo.regla.nombre;
                string consecuentes = "";
                if (nodo.nodosSiguientes.Count == 0)
                {
                    continue;
                }
                foreach (NodoRegla consecuente in nodo.nodosSiguientes)
                {
                    consecuentes = consecuentes + consecuente.regla.nombre;
                    consecuentes = !consecuente.Equals(nodo.nodosSiguientes.Last()) ? consecuentes + ", " : consecuentes;
                }
                Console.WriteLine(nombre + " -> " + consecuentes);
            }
        }

    }
}
