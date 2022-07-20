using System;
using System.Collections.Generic;
using System.Text;

namespace Sistema_de_Asesoramiento_legal_en_el_área_del_trabajo
{
    class ArbolDeHechos
    {
        public List<Regla> listaDeReglas;
        public List<NodoHecho> hojas;
        public NodoHecho raiz;
        private Regla reglaRaiz;
        private List<NodoHecho> nodosCreados;

        public ArbolDeHechos(List<Regla> listaDeReglas)
        {
            this.listaDeReglas = new List<Regla>();
            this.listaDeReglas.AddRange(listaDeReglas);
            this.hojas = new List<NodoHecho>();
            this.nodosCreados = new List<NodoHecho>();
            raiz = new NodoHecho();
            nodosCreados.Add(raiz);
            crearArbol();
            //imprimirArbol(raiz);
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
                        raiz.hecho.nombre = regla.nombre;
                        break;
                    }
                }
            }
        }

        private void crearArbol()
        {
            encontrarReglaRaiz();
            raiz.nivel = 0;
            List<NodoHecho> nodosNivelAnterior = new List<NodoHecho>();
            nodosNivelAnterior.Add(raiz);
            encontrarNodos(raiz,reglaRaiz, nodosNivelAnterior);
            asignarNiveles(raiz, 0);
        }

        private void encontrarNodos(NodoHecho nodo, Regla reglaNodo, List<NodoHecho> nodosNivelAnterior)
        {
            nodo.nodosSiguientes.AddRange(encontrarNodosSiguientes(nodo, reglaNodo,nodosNivelAnterior));
        }

        private List<NodoHecho> encontrarNodosSiguientes(NodoHecho nodo, Regla reglaNodo, List<NodoHecho> nodosNivelAnterior)
        {
            List<NodoHecho> nodosSiguientesEncontrados = new List<NodoHecho>();
            List<Regla> reglasNodosSiguientes = new List<Regla>();
            List<NodoHecho> nodosExistentes = new List<NodoHecho>();
            //nodo.nodosAnteriores.AddRange(encontrarNodosAnteriores(nodo,reglaNodo,nodosNivelAnterior));

            foreach (Regla regla in listaDeReglas)
            {
                foreach (string premisa in regla.premisas)
                {
                    if (premisa.Equals(nodo.hecho.nombre))
                    {
                        NodoHecho nodoSiguienteExistente = new NodoHecho();
                        if (regla.premisas.Count > 1)
                        {
                            nodoSiguienteExistente = buscarNodo(regla);
                            if (!nodoSiguienteExistente.hecho.nombre.Equals(""))
                            {
                                nodosSiguientesEncontrados.Add(nodoSiguienteExistente);
                                nodosExistentes.Add(nodoSiguienteExistente);
                                reglasNodosSiguientes.Add(regla);
                                break;
                            }
                        }
                        NodoHecho nodoSgt = new NodoHecho();
                        nodoSgt.hecho.nombre = regla.nombre;
                        nodoSgt.hecho.nivelesVerdadNecesarios = regla.nivelesDeVerdadNecesarios;
                        nodosSiguientesEncontrados.Add(nodoSgt);
                        reglasNodosSiguientes.Add(regla);
                        nodosCreados.Add(nodoSgt);
                        nodoSgt.nodosAnteriores.AddRange(encontrarNodosAnteriores(nodoSgt, regla, nodosNivelAnterior));
                    }
                }
            }
            int contador = 0;
            foreach (NodoHecho nodoSiguiente in nodosSiguientesEncontrados)
            {
                if (!nodosExistentes.Contains(nodoSiguiente))
                {
                    encontrarNodos(nodoSiguiente, reglasNodosSiguientes[contador], nodosSiguientesEncontrados);
                }
                contador++;
            }
            return nodosSiguientesEncontrados;
        }

        private NodoHecho buscarNodo(Regla regla)
        {
            NodoHecho nodoSiguienteExistente = new NodoHecho();
            foreach(NodoHecho nodoCreado in nodosCreados)
            {
                int contador = -1;
                if (mismoNombre(regla, nodoCreado))
                {
                    contador = 0;
                    foreach (NodoHecho nodoAnterior in nodoCreado.nodosAnteriores)
                    {
                        foreach (string premisa in regla.premisas)
                        {
                            if (nodoAnterior.hecho.nombre.Equals(premisa))
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

        private bool mismoNombre(Regla regla, NodoHecho nodo)
        {
            return (nodo.hecho.nombre.Equals(regla.nombre));
        }

        private List<NodoHecho> encontrarNodosAnteriores(NodoHecho nodo, Regla reglaNodo, List<NodoHecho> nodosNivelAnterior)
        {
            List<NodoHecho> nodosAnteriores = new List<NodoHecho>();
            foreach(NodoHecho nodoPadre in nodosNivelAnterior)
            {
                foreach(string premisa in reglaNodo.premisas)
                {
                    if (nodoPadre.hecho.nombre.Equals(premisa))
                    {
                        nodosAnteriores.Add(nodoPadre);
                    }
                }
            }
            return nodosAnteriores;
        }

        private void asignarNiveles(NodoHecho nodo, int nivel)
        {
            nodo.nivel = nivel;
            if (existeNodoSiguiente(nodo))
            {
                foreach (NodoHecho nodoSgt in nodo.nodosSiguientes)
                {
                    asignarNiveles(nodoSgt, (nivel + 1));
                }
            }
            else
            {
                agregarNodoHoja(nodo);
            }
        }

        private bool existeNodoSiguiente(NodoHecho nodo)
        {
            if (nodo.nodosSiguientes.Count != 0)
            {
                return true;
            }
            return false;
        }
        private void agregarNodoHoja(NodoHecho nodo)
        {
            hojas.Add(nodo);
        }

        public void imprimirArbol(NodoHecho nodo)
        {
            Console.WriteLine(nodo.ToString());
            if (existeNodoSiguiente(nodo))
            {
                foreach (NodoHecho nodoSgt in nodo.nodosSiguientes)
                {
                    imprimirArbol(nodoSgt);
                }
            }
        }

    }


}
