using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Sistema_de_Asesoramiento_legal_en_el_área_del_trabajo
{
    class MotorDeInferencia
    {
        List<List<(NodoHecho,NodoRegla)>> nodosRecorridos;
        public List<NodoRegla> conclusiones;
        NodoRegla reglaAplicable;
        BaseDeReglas baseDeReglas;
        BaseDeHechos baseDeHechos;
        Fuzzificador fuzzificador;
        bool deseaSeguir, quedanReglasAplicables;
        int indiceArbol;
        List<int> cantidadPreguntasNecesariasAux;

        public MotorDeInferencia(BaseDeReglas baseDeReglas, BaseDeHechos baseDeHechos)
        {
            conclusiones = new List<NodoRegla>();
            cantidadPreguntasNecesariasAux = new List<int>();
            reglaAplicable = new NodoRegla();
            this.baseDeReglas = baseDeReglas;
            this.baseDeHechos = baseDeHechos;
            nodosRecorridos = crearListaDeNodosRecorridos();
            fuzzificador = new Fuzzificador();
            deseaSeguir = true;
            quedanReglasAplicables = true;
            indiceArbol = 0;
        }

        private List<List<(NodoHecho, NodoRegla)>> crearListaDeNodosRecorridos()
        {
            List <List<(NodoHecho, NodoRegla)>> lista = new List<List<(NodoHecho, NodoRegla)>>();
            foreach(ArbolDeHechos arbol in baseDeHechos.arbolesDeHechos)
            {
                lista.Add(new List<(NodoHecho, NodoRegla)>());
            }
            return lista;
        }

        public void encadenarHaciaAdelante()
        {
            while ((quedanReglasAplicables && deseaSeguir))
            {
                if((existeReglaAplicable() && deseaSeguir))
                {
                    agregarNuevoHecho(reglaAplicable);
                }
            }
        }

        private void agregarNuevoHecho(NodoRegla reglaAplicable)
        {
            NodoHecho nodoHechoPreguntar = obtenerUltimoNodoHechoRecorrido();
            NodoRegla nodoReglaPreguntar = obtenerUltimoNodoReglaRecorrido();
            marcarRecorrido(nodoHechoPreguntar);

            preguntar(nodoReglaPreguntar.regla.pregunta);
            string respuesta = leerRespuesta();
            float certeza = obtenerValorCerteza(respuesta);
            propagarCerteza(nodoHechoPreguntar, certeza);
            Console.Clear();
        }

        private void marcarRecorrido(NodoHecho nodoHecho)
        {
            nodoHecho.recorrido = true;
        }

        private NodoRegla obtenerUltimoNodoReglaRecorrido()
        {
            return nodosRecorridos[indiceArbol][nodosRecorridos[indiceArbol].Count - 1].Item2;
        }

        private NodoHecho obtenerUltimoNodoHechoRecorrido()
        {
            return nodosRecorridos[indiceArbol][nodosRecorridos[indiceArbol].Count - 1].Item1;
        }

        private void preguntarContinuar()
        {
            imprimir("¿Desea continuar con la ejecucion?");
            imprimir("\nEscoja una opcion (1 - 2):" +
                   "\n1.Si" +
                   "\n2.No");
            Regex rx = new Regex("^[1-2]$");
            string respuesta = "";
            while (true)
            {
                respuesta = Console.ReadLine();
                if ((rx.IsMatch(respuesta)))
                {
                    break;
                }
                imprimir("\nRespuesta no valida, utilice una de las opciones (1 - 2)");
            }
            deseaSeguir = (respuesta.Equals("2")) ? false:true;
            Console.Clear();
        }

        private void preguntar(string pregunta)
        {
            imprimir(pregunta);
            imprimir("\nEn una escala desde 0 hasta 1, siendo 0 falso, 0.5 inseguro y 1 verdadero, con un maximo de 6 decimales" +
                "\n¿Que tan cierto es la situacion descrita en la pregunta?");
        }

        private string leerRespuesta()
        {
            string stringCerteza = "";
            Regex rx = new Regex(@"^([0]|[0]\,[0-9]{1,6}|[1]|1.0)$");
            while (true)
            {
                stringCerteza = Console.ReadLine();
                if (rx.IsMatch(stringCerteza))
                {
                    break;
                }
                imprimir("\nRespuesta no valida, indique en una escala de 0 a 1");
            }
            return stringCerteza;
        }

        private float obtenerValorCerteza(string nivelCerteza)
        {
            return float.Parse(nivelCerteza);
        }

        private NodoHecho obtenerNodoRaizArbolHechoActual()
        {
            return baseDeHechos.arbolesDeHechos[indiceArbol].raiz;
        }

        private NodoHecho encontrarNodoHechoEquivalente(NodoRegla reglaAplicable, NodoHecho nodoHechoCandidato)
        {
            int cantidadNodosAnterioresNodoHecho = nodoHechoCandidato.nodosAnteriores.Count;
            NodoHecho nodoEquivalente = new NodoHecho();
            if (obtenerNombresIgualesNodosAnteriores(reglaAplicable, nodoHechoCandidato) != cantidadNodosAnterioresNodoHecho)
            {
                foreach (NodoHecho nodoHechoSiguiente in nodoHechoCandidato.nodosSiguientes)
                {
                    nodoEquivalente = encontrarNodoHechoEquivalente(reglaAplicable,nodoHechoSiguiente);
                }
            }
            else 
            {
                nodoEquivalente = nodoHechoCandidato; 
            }
            return nodoEquivalente;
        }

        private int obtenerNombresIgualesNodosAnteriores(NodoRegla nodoR, NodoHecho nodoHechoCandidato)
        {
            int contador = -1;
            if (nodoHechoCandidato.hecho.nombre.Equals(nodoR.regla.nombre))
            {
                contador = 0;
                foreach (NodoHecho nodoHechoAnterior in nodoHechoCandidato.nodosAnteriores)
                {
                    foreach (NodoRegla nodoRegla in nodoR.nodosAnteriores)
                    {
                        contador = (nodoHechoAnterior.hecho.nombre.Equals(nodoRegla.regla.nombre)) ? contador+1: contador;
                    }
                }
            }
            return contador;
        }

        private bool existeReglaAplicable()
        {
            bool existeRegla = false;
            if (!llegoUltimoArbol())
            {
                NodoHecho nodoDeBusqueda = obtenerNodoDeBusqueda();
                List<(NodoHecho,NodoRegla)> reglasAplicables = new List<(NodoHecho,NodoRegla)>();
                buscarReglasAplicables(nodoDeBusqueda, reglasAplicables);
                deducirHechos(reglasAplicables);
                reglaAplicable = escogerReglaAplicable(reglasAplicables);
                mensajeDebug("Nodo de inicio de busqueda regla aplicable: " + nodoDeBusqueda.hecho.nombre);
                if (string.IsNullOrEmpty(reglaAplicable.regla.nombre))
                {
                    aumentarIndiceArbol();
                    mensajeDebug("No se ha encontrado regla y se ha aumentado el indice del arbol");
                }
                else
                {
                    mensajeDebug("Se ha encontrado regla: " + reglaAplicable.regla.ToString());
                    existeRegla = true;
                }
            }
            else
            {
                imprimir("Ya no quedan reglas aplicables");
                quedanReglasAplicables = false;
            }
            return existeRegla;
        }

        private void deducirHechos(List<(NodoHecho, NodoRegla)> reglasAplicables)
        {
            List<(NodoHecho, NodoRegla)> conclusionesEncontradas = new List<(NodoHecho, NodoRegla)>();
            foreach ((NodoHecho, NodoRegla) datosReglaAplicable in reglasAplicables)
            {
                NodoHecho hechoAplicable = datosReglaAplicable.Item1;
                NodoRegla reglaAplicable = datosReglaAplicable.Item2;
                if (!tieneSiguientes(hechoAplicable))
                {
                    imprimir("Se ha llegado a una conclusión ( " + reglaAplicable.regla.nombre + " )");
                    marcarRecorrido(hechoAplicable);
                    propagarCerteza(hechoAplicable, 1);
                    agregarConclusion(reglaAplicable);
                    preguntarContinuar();
                    conclusionesEncontradas.Add(datosReglaAplicable);
                }
            }
            limpiarReglasAplicables(reglasAplicables,conclusionesEncontradas);
        }

        private void limpiarReglasAplicables(List<(NodoHecho, NodoRegla)> reglasAplicables, List<(NodoHecho, NodoRegla)> conclusionesEncontradas)
        {
            foreach((NodoHecho, NodoRegla) conclusion in conclusionesEncontradas)
            {
                reglasAplicables.Remove(conclusion);
            }
        }

        private bool tienePregunta(NodoRegla reglaAplicable)
        {
            return (!string.IsNullOrEmpty(reglaAplicable.regla.pregunta));
        }

        private bool llegoUltimoArbol()
        {
            return (indiceArbol >= baseDeHechos.arbolesDeHechos.Count);
        }

        private void aumentarIndiceArbol()
        {
            indiceArbol++;
        }

        private NodoRegla escogerReglaAplicable(List<(NodoHecho, NodoRegla)> reglasAplicables)
        {
            List<(NodoHecho, NodoRegla)> reglasOptimas = new List<(NodoHecho, NodoRegla)>();
            if (reglasAplicables.Count!=0)
            {
                reglasOptimas = obtenerReglasMenorCantidad(reglasAplicables);
                reglasOptimas = obtenerReglasMayorProfundidad(reglasOptimas);

                if (!(string.IsNullOrEmpty(reglasOptimas[0].Item2.regla.nombre)))
                {
                    nodosRecorridos[indiceArbol].Add((reglasOptimas[0].Item1, reglasOptimas[0].Item2));
                }
                return reglasOptimas[0].Item2;
            }
            return new NodoRegla();
        }

        private List<(NodoHecho, NodoRegla)> obtenerReglaMasCertera(List<(NodoHecho, NodoRegla)> reglasOptimas)
        {
            List<(NodoHecho, NodoRegla)> datosMasCerteros = new List<(NodoHecho, NodoRegla)>();
            NodoHecho hechoMasCertero = new NodoHecho();
            NodoRegla reglaMasCertera = new NodoRegla();
            float mayorCerteza = 0;

            foreach ((NodoHecho, NodoRegla) datosRegla in reglasOptimas)
            {
                if (datosRegla.Item1.hecho.certeza > mayorCerteza)
                {
                    hechoMasCertero = datosRegla.Item1;
                    reglaMasCertera = datosRegla.Item2;
                }
            }
            datosMasCerteros.Add((hechoMasCertero,reglaMasCertera));
            return datosMasCerteros;
        }

        private List<(NodoHecho, NodoRegla)> obtenerReglasMayorProfundidad(List<(NodoHecho, NodoRegla)> reglasOptimas)
        {
            List<(NodoHecho, NodoRegla)> reglasMayorProfundidad = new List<(NodoHecho, NodoRegla)>();
            int mayorProfundidad = obtenerMayorProfundidad(reglasOptimas);
            //Console.WriteLine("Mayor profundidad = "+mayorProfundidad);
            foreach((NodoHecho, NodoRegla) reglaAplicable in reglasOptimas)
            {
                if (reglaAplicable.Item1.nivel == mayorProfundidad)
                {
                    //Console.WriteLine("Se añadio la regla"+ reglaAplicable.Item1.hecho.nombre +" con nivel= "+reglaAplicable.Item1.nivel);
                    reglasMayorProfundidad.Add(reglaAplicable);
                }
            }
            return reglasMayorProfundidad;
        }

        private int obtenerMayorProfundidad(List<(NodoHecho, NodoRegla)> reglasOptimas)
        {
            int mayorProfundidad = 0;
            foreach ((NodoHecho, NodoRegla) reglaAplicable in reglasOptimas)
            {
                if (reglaAplicable.Item1.nivel > mayorProfundidad)
                {
                    mayorProfundidad = reglaAplicable.Item1.nivel;
                }
            }
            return mayorProfundidad;
        }

        private List<(NodoHecho, NodoRegla)> obtenerReglasMenorCantidad(List<(NodoHecho, NodoRegla)> reglasAplicables)
        {
            List<(NodoHecho, NodoRegla)> reglasMenorCantidadPreguntas = new List<(NodoHecho, NodoRegla)>();
            List<int> preguntasNecesarias = new List<int>();
            int menorCantidadPreguntasNecesarias;
            preguntasNecesarias = enlistarCantidadesPreguntasNecesarias(reglasAplicables);
            menorCantidadPreguntasNecesarias = obtenerMenorCantidad(preguntasNecesarias);
            for (int contador = 0; contador < reglasAplicables.Count; contador++)
            {
                if (preguntasNecesarias[contador] == menorCantidadPreguntasNecesarias)
                {
                    //Console.WriteLine("Regla mas aplicable-> " + reglasAplicables[contador].Item2.regla.nombre + " con una cantidad de " + menorCantidadPreguntasNecesarias);
                    reglasMenorCantidadPreguntas.Add((reglasAplicables[contador].Item1, reglasAplicables[contador].Item2));
                }
            }
            return reglasMenorCantidadPreguntas;
        }

        private int obtenerMenorCantidad(List<int> preguntasNecesarias)
        {
            int menorCantidad = preguntasNecesarias[0];
            for (int contador = 0; contador < preguntasNecesarias.Count; contador++)
            {
                if (preguntasNecesarias[contador] <= menorCantidad)
                {
                    menorCantidad = preguntasNecesarias[contador];
                }
            }
            return menorCantidad;
        }

        private List<int> enlistarCantidadesPreguntasNecesarias(List<(NodoHecho, NodoRegla)> reglasAplicables)
        {
            List<int> cantidadPreguntasNecesarias = new List<int>();
            foreach((NodoHecho, NodoRegla) reglaAplicable in reglasAplicables)
            {
                cantidadPreguntasNecesariasAux.Clear();
                //Console.WriteLine("\nPara "+reglaAplicable.Item1.hecho.nombre);
                encontrarCantidadPreguntasNecesarias(reglaAplicable.Item1, 0);
                //Console.WriteLine("Al final se obtuvo " + escogerMenorCantidad()+" para "+ reglaAplicable.Item1.hecho.nombre+"\n");
                cantidadPreguntasNecesarias.Add(escogerMenorCantidad());
            }
            return cantidadPreguntasNecesarias;
        }

        private int escogerMenorCantidad()
        {
            int menorCantidad = cantidadPreguntasNecesariasAux[0];
            foreach(int cantidad in cantidadPreguntasNecesariasAux)
            {
                if (cantidad < menorCantidad)
                {
                    menorCantidad = cantidad;
                }
            }
            return menorCantidad;
        }

        private void encontrarCantidadPreguntasNecesarias(NodoHecho nodo, int cantidadPreguntasNecesarias)
        {
            if (nodo.nodosSiguientes.Count != 0)
            {
                cantidadPreguntasNecesarias++;
            }
            else
            {
                //Console.WriteLine("Se llegó a una hoja ("+nodo.hecho.nombre+") y se debe preguntar "+cantidadPreguntasNecesarias+" veces");
                cantidadPreguntasNecesariasAux.Add(cantidadPreguntasNecesarias);
            }
            foreach (NodoHecho nodoSiguiente in nodo.nodosSiguientes)
            {
                encontrarCantidadPreguntasNecesarias(nodoSiguiente, cantidadPreguntasNecesarias);
            }
        }

        private void mensajeDebug(string v)
        {
            //Console.WriteLine("\n*|*DEBUG*|*\n"+"v+ "\n*|*DEBUG*|*\n");
        }

        private NodoHecho obtenerNodoDeBusqueda()
        {
            return obtenerNodoRaizArbolHechoActual();
        }

        private void buscarReglasAplicables(NodoHecho nodoBuscado, List<(NodoHecho,NodoRegla)> reglasAplicables)
        {
            NodoRegla nodoReglaEquivalente = new NodoRegla();
            if (esAplicable(nodoBuscado))
            {
                NodoRegla nodoRaiz = obtenerNodoRaizArbolReglaActual();
                nodoReglaEquivalente = encontrarNodoEquivalente(nodoBuscado, nodoRaiz);
                reglasAplicables.Add((nodoBuscado, nodoReglaEquivalente));
                mensajeDebug("Se ha añadido hecho: " + nodoBuscado.hecho.ToString() + "\n Y Regla: " + nodoReglaEquivalente.regla.ToString());
            }
            else
            {
                foreach(NodoHecho nodoSiguiente in nodoBuscado.nodosSiguientes)
                {
                    buscarReglasAplicables(nodoSiguiente, reglasAplicables);
                }
            }
        }

        private void imprimir(string mensaje)
        {
            Console.WriteLine(mensaje);
        }

        private bool tieneSiguientes(NodoHecho nodoBuscado)
        {
            return (nodoBuscado.nodosSiguientes.Count > 0);
        }

        private bool esAplicable(NodoHecho nodoBuscado)
        {
            return (!recorrido(nodoBuscado) && (!tieneAnteriores(nodoBuscado) | premisasRecorridas(nodoBuscado)) && nivelesVerdadNecesarios(nodoBuscado));
        }

        private bool recorrido(NodoHecho nodoBuscado)
        {
            return nodoBuscado.recorrido;
        }

        private bool nivelesVerdadNecesarios(NodoHecho nodoBuscado)
        {
            List<string> nivelesFalsos = new List<string> { "Falso", "Probablemente falso", "Inseguro" };
            List<string> nivelesVerdaderos = new List<string> { "Verdadero", "Probablemente verdadero", "Inseguro" };
            bool nivelesVerdadSatisfacidos = true;
            int contador = 0;
            foreach(string nivelVerdadNecesario in nodoBuscado.hecho.nivelesVerdadNecesarios)
            {
                if (nivelVerdadNecesario.Equals("V"))
                {
                    if (!nivelesVerdaderos.Contains(nodoBuscado.nodosAnteriores[contador].hecho.nivelCerteza))
                    {
                        nivelesVerdadSatisfacidos = false;
                    }
                }
                else
                {
                    if (!nivelesFalsos.Contains(nodoBuscado.nodosAnteriores[contador].hecho.nivelCerteza))
                    {
                        nivelesVerdadSatisfacidos = false;
                    }
                }
                contador++;
            }
            return nivelesVerdadSatisfacidos;
        }

        private bool premisasRecorridas(NodoHecho nodoBuscado)
        {
            foreach (NodoHecho nodoPadre in nodoBuscado.nodosAnteriores)
            {
                if(nodoPadre.recorrido == false)
                {
                    return false;
                }
            }
            return true;
        }

        private NodoRegla obtenerNodoRaizArbolReglaActual()
        {
            return baseDeReglas.arbolesDeReglas[indiceArbol].raiz;
        }

        private void agregarConclusion(NodoRegla nodoReglaEquivalente)
        {
            conclusiones.Add(nodoReglaEquivalente);
        }

        private NodoRegla encontrarNodoEquivalente(NodoHecho nodoBuscado, NodoRegla nodoDondeSeBusca)
        {
            NodoRegla nodoEncontrado = new NodoRegla();
            if (mismasPremisas(nodoDondeSeBusca,nodoBuscado))
            {
                nodoEncontrado = nodoDondeSeBusca;
            }
            else
            {
                foreach (NodoRegla nodoSiguiente in nodoDondeSeBusca.nodosSiguientes)
                {
                    nodoEncontrado = encontrarNodoEquivalente(nodoBuscado, nodoSiguiente);
                    if(!(nodoEncontrado.nodosAnteriores.Count == 0 && nodoEncontrado.nodosSiguientes.Count == 0))
                    {
                        break;
                    }
                }
            }
            return nodoEncontrado;
        }

        private bool mismasPremisas(NodoRegla nodoR, NodoHecho nodoH)
        {
            int cantidadNodosAnterioresNodoHecho = nodoH.nodosAnteriores.Count;
            return (obtenerNombresIgualesNodosAnteriores(nodoR, nodoH) == cantidadNodosAnterioresNodoHecho);
        }

        private void propagarCerteza(NodoHecho nodoHechoActual, float certeza)
        {
            /*if (tieneAnteriores(nodoHechoActual))
            {
                float certezaAnterior = obtenerMenorCertezaNodosAnteriores(nodoHechoActual);
                certeza = (certezaAnterior < certeza) ? certezaAnterior : certeza;
            }*/
            asignarCerteza(nodoHechoActual, certeza);
            asignarNivelDeCerteza(nodoHechoActual, certeza);
            /*foreach(NodoHecho nodoHecho in nodoHechoActual.nodosSiguientes)
            {
                propagarCerteza(nodoHecho, certeza);
            }*/
        }

        private void asignarNivelDeCerteza(NodoHecho nodoHechoActual, float certeza)
        {
            nodoHechoActual.hecho.nivelCerteza = fuzzificador.fuzzificar(certeza);
            /*Console.WriteLine("El hecho: " + nodoHechoActual.hecho.nombre + " recae dentro de el nivel: " + nodoHechoActual.hecho.nivelCerteza);
            Console.ReadLine();*/
        }

        private bool tieneAnteriores(NodoHecho nodoHechoActual)
        {
            return (nodoHechoActual.nodosAnteriores.Count > 0);
        }

        private float obtenerMenorCertezaNodosAnteriores(NodoHecho nodoHechoActual)
        {
            float certeza = 2;
            foreach(NodoHecho nodo in nodoHechoActual.nodosAnteriores)
            {
                certeza = (nodo.hecho.certeza < certeza) ? nodo.hecho.certeza : certeza;
            }
            return certeza;
        }

        private void asignarCerteza(NodoHecho nodoHechoActual, float certeza)
        {
            nodoHechoActual.hecho.certeza = certeza;
        }

    }
}