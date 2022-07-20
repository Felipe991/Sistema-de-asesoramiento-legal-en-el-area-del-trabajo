using System;
using System.Collections.Generic;
using System.Text;

namespace Sistema_de_Asesoramiento_legal_en_el_área_del_trabajo
{
    class Regla
    {
        public string nombre;
        public string pregunta;
        public string explicacion;
        public List<string> premisas;
        public List<string> nivelesDeVerdadNecesarios;
        public string indicaciones;
        

        public Regla(string nombre, string pregunta, string explicacion, List<string> premisas, string indicaciones, List<string> nivelesDeVerdadNecesarios)
        {
            this.nombre = nombre;
            this.pregunta = pregunta;
            this.explicacion = explicacion;
            this.premisas = new List<string>();
            this.nivelesDeVerdadNecesarios = new List<string>();
            this.nivelesDeVerdadNecesarios.AddRange(nivelesDeVerdadNecesarios);
            this.premisas.AddRange(premisas);
            this.indicaciones = indicaciones;
        }

        public Regla()
        {
            this.nombre = "";
        }

        public override string ToString()
        {
            return "\nNombre: "+ nombre + "  "+
                   "\nPregunta: " + pregunta + "  "+
                   "\nExplicacion: " + explicacion + "  "+
                   "\nPremisas: " + String.Join(", ", premisas.ToArray())+
                   "\nIndicaciones: "+ indicaciones;
        }
    }
}
