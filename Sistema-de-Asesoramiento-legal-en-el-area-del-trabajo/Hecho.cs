using System;
using System.Collections.Generic;
using System.Text;

namespace Sistema_de_Asesoramiento_legal_en_el_área_del_trabajo
{
    class Hecho
    {
        public string nombre;
        public float certeza;
        public string nivelCerteza;
        public List<string> nivelesVerdadNecesarios;

        public Hecho()
        {
            nombre = "";
            nivelCerteza = "Verdadero";
            certeza = 1;
            nivelesVerdadNecesarios = new List<string>();
    }

        public override string ToString()
        {
            return "\nNombre: " + nombre +
                   "\nCerteza: " + certeza+
                   "\nNivel certeza: "+nivelCerteza;
        }
    }
}
