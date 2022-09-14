namespace apimodels.models
{
    public class Maquete
    {
        public Maquete()
        {
        }

        public Maquete(int id, string nomeMaquete, string descricaoMaquete, string imgMaquete, string arquivoMaquete, bool maqueteAtiva, int idUsuario, DateTimeOffset? dataPublicacao)
        {
            Id = id;
            NomeMaquete = nomeMaquete;
            DescricaoMaquete = descricaoMaquete;
            ImgMaquete = imgMaquete;
            ArquivoMaquete = arquivoMaquete;
            MaqueteAtiva = maqueteAtiva;
            IdUsuario = idUsuario;
            DataPublicacao = dataPublicacao;
        }

        public int Id { get; set; }
        public string NomeMaquete { get; set; }
        public string DescricaoMaquete { get; set; }
        public string ImgMaquete { get; set; }
        public string ArquivoMaquete { get; set; }
        public bool MaqueteAtiva { get; set; }
        public int IdUsuario { get; set; }
        public DateTimeOffset? DataPublicacao { get; set; }
    }
}