using System;
using System.Collections.Generic;
using System.Text;

namespace Sistema_de_Asesoramiento_legal_en_el_área_del_trabajo
{
    class BaseDeReglas
    {
        public List<ArbolDeReglas> arbolesDeReglas;

        public BaseDeReglas(List<List<Regla>> listaDeListasDeReglas)
        {
            arbolesDeReglas = new List<ArbolDeReglas>();
            foreach (List<Regla> listaDeRegla in listaDeListasDeReglas)
            {
                arbolesDeReglas.Add(new ArbolDeReglas(listaDeRegla));
            }
        }
    }
}
