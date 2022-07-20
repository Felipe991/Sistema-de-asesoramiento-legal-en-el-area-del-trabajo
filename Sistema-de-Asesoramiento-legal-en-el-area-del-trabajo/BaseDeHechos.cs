using System;
using System.Collections.Generic;
using System.Text;

namespace Sistema_de_Asesoramiento_legal_en_el_área_del_trabajo
{
    class BaseDeHechos
    {
        public List<ArbolDeHechos> arbolesDeHechos;

        public BaseDeHechos(List<List<Regla>> listaDeListasDeReglas)
        {
            arbolesDeHechos = new List<ArbolDeHechos>();
            foreach (List<Regla> listaDeRegla in listaDeListasDeReglas)
            {
                arbolesDeHechos.Add(new ArbolDeHechos(listaDeRegla));
            }
        }
    }
}
