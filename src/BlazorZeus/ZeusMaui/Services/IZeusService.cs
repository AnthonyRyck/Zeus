using ZeusCore;

namespace ZeusMaui.Services
{
	public interface IZeusService
	{
		/// <summary>
		/// Adresse du serveur.
		/// </summary>
		string UrlServer { get; }

		/// <summary>
		/// Permet de récupérer la liste des films
		/// </summary>
		/// <returns></returns>
		Task<List<InformationMovie>> GetAllMovies();
		
		/// <summary>
		/// Récupère les informations d'un film.
		/// </summary>
		/// <param name="idMovie"></param>
		/// <returns></returns>
		Task<DetailMovie> GetMovie(Guid idMovie);

		/// <summary>
		/// Test l'adresse du serveur.
		/// </summary>
		/// <param name="adresseServer"></param>
		/// <returns></returns>
		Task<bool> TestServerUrl(string adresseServer);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="addressServer"></param>
		void ChangeServerAddress(string addressServer);

		/// <summary>
		/// Sauvegarde l'adresse du serveur.
		/// </summary>
		/// <param name="adresseSvr"></param>
		Task SaveServeur(string adresseSvr);
	}
}
