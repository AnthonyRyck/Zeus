using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MoviesLib.Entities;
using TMDbLib.Objects.TvShows;
using WebAppServer.Models;

namespace WebAppServer.Codes
{
    public class SerieCollection : IList<ShowModel>
    {
		#region Properties

		private readonly IList<ShowModel> _list = new List<ShowModel>();
		private static readonly Object _objectToLock = new object();

		/// <summary>
		/// Propriété utilisé pour savoir ce qui est ajouté.
		/// </summary>
		private List<KeyValuePair<TvSeason, TvEpisode>> _nouveautes = new List<KeyValuePair<TvSeason, TvEpisode>>();

		#endregion
		
		#region Implementation of IEnumerable

		public IEnumerator<ShowModel> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Implementation of ICollection<ShowModel>

        public void Add(ShowModel item)
        {
            _list.Add(item);

			if (item.TvEpisodes.Count == 1 && item.TvSeasons.Count == 1)
			{
				AddNouveaute(item.TvSeasons.First(), item.TvEpisodes.First());
			}
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(ShowModel item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(ShowModel[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public bool Remove(ShowModel item)
        {
            return _list.Remove(item);
        }

        public int Count
        {
            get { return _list.Count; }
        }

        public bool IsReadOnly
        {
            get { return _list.IsReadOnly; }
        }

        #endregion

        #region Implementation of IList<ShowModel>

        public int IndexOf(ShowModel item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, ShowModel item)
        {
            _list.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        public ShowModel this[int index]
        {
            get { return _list[index]; }
            set { _list[index] = value; }
        }

		#endregion
		
		#region Custom methods

		/// <summary>
		/// Retourne la liste des ShowModel.
		/// </summary>
		/// <returns></returns>
	    public List<ShowModel> Get()
	    {
		    return _list.ToList();
	    }

		/// <summary>
		/// Permet de mettre à jour la liste des séries.
		/// </summary>
		/// <param name="collectionShowModels"></param>
	    public void Set(IEnumerable<ShowModel> collectionShowModels)
		{
			_list.Clear();
			foreach (ShowModel model in collectionShowModels)
			{
				_list.Add(model);
			}
		}

		/// <summary>
		/// Récupére l'ID du showModel en fonction d'un titre.
		/// </summary>
		/// <param name="title"></param>
		/// <returns></returns>
		public Guid GetIdShow(string title)
	    {
			foreach (ShowModel model in _list)
		    {
			    if (model.TvShow != null 
					&& IsSameShow(model.TvShow.Name, title))
			    {
				    return model.IdShowModel;
			    }
		    }

		    return Guid.Empty;
		}

	    /// <summary>
	    /// Ajoute un épisode à la série donnée par l'id.
	    /// </summary>
	    /// <param name="idShow"></param>
	    /// <param name="episode"></param>
	    /// <param name="serieLocal"></param>
	    public void AddEpisode(Guid idShow, TvEpisode episode, ShowInformation serieLocal)
	    {
		    ShowModel showModel = GetShowModel(idShow);
			showModel.TvEpisodes.Add(episode);
			showModel.ShowInformation.Add(serieLocal);

			AddNouveaute(showModel.TvSeasons.Where(x => x.SeasonNumber == episode.SeasonNumber).First(), episode);
		}

		/// <summary>
		/// Ajoute la saison et un épisode à la série donnée par l'id.
		/// </summary>
		/// <param name="idShow"></param>
		/// <param name="saison"></param>
		/// <param name="episode"></param>
		/// <param name="serieLocal"></param>
		public void AddSaison(Guid idShow, TvSeason saison, TvEpisode episode, ShowInformation serieLocal)
	    {
			ShowModel showModel = GetShowModel(idShow);
			showModel.TvSeasons.Add(saison);
		    showModel.TvEpisodes.Add(episode);
		    showModel.ShowInformation.Add(serieLocal);

			AddNouveaute(saison, episode);
		}

		/// <summary>
		/// Retourne une indication si cette saison est connu sur la série donnée
		/// par l'ID.
		/// </summary>
		/// <param name="idShow"></param>
		/// <param name="numberSeason"></param>
		/// <returns></returns>
		public bool HaveSeason(Guid idShow, short numberSeason)
	    {
		    ShowModel showModel = GetShowModel(idShow);
		    return showModel.HaveSeason(numberSeason);
	    }

		/// <summary>
		/// Retourne une indication si cet épisode de la saison donnée, pour une série
		/// donnée par l'ID est connu.
		/// </summary>
		/// <param name="idShow"></param>
		/// <param name="seasonNumber"></param>
		/// <param name="episodeNumber"></param>
		/// <returns></returns>
	    public bool HaveEpisode(Guid idShow, short seasonNumber, short episodeNumber)
	    {
		    ShowModel showModel = GetShowModel(idShow);
		    return showModel.HaveEpisode(seasonNumber, episodeNumber);
	    }

		/// <summary>
		/// Retourne l'ID de TmDb pour cette série donnée par le Guid.
		/// </summary>
		/// <param name="idShow"></param>
		/// <returns></returns>
	    public int GetIdSerieTmDb(Guid idShow)
		{
			ShowModel show = GetShowModel(idShow);
			return show.TvShow.Id;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="showInformationsOnLocal"></param>
	    public void UpdateShowInformations(IEnumerable<ShowInformation> showInformationsOnLocal)
	    {
			List<KeyValuePair<Guid,ShowInformation>> listToDelete = new List<KeyValuePair<Guid, ShowInformation>>();
			List<KeyValuePair<Guid, ShowInformation>> allShowInformations = new List<KeyValuePair<Guid, ShowInformation>>();

		    foreach (ShowModel model in _list)
		    {
			    Guid key = model.IdShowModel;

			    foreach (ShowInformation information in model.ShowInformation)
			    {
				    allShowInformations.Add(new KeyValuePair<Guid, ShowInformation>(key, information));
			    }
		    }

		    foreach (KeyValuePair<Guid, ShowInformation> showInformation in allShowInformations)
		    {
				if (!showInformationsOnLocal.Contains(showInformation.Value))
				{
					listToDelete.Add(showInformation);
				}
			}

		    foreach (KeyValuePair<Guid, ShowInformation> itemToDelete in listToDelete)
		    {
			    ShowModel model = GetShowModel(itemToDelete.Key);
				model.RemoveVideo(itemToDelete.Value);
		    }

		    UpdateShowModel();
	    }

		/// <summary>
		/// Récupère le ShowModel par rapport à l'ID.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public ShowModel GetShowModel(Guid id)
		{
			return _list.FirstOrDefault(x => x.IdShowModel == id);
		}

		/// <summary>
		/// Retourne toutes les nouveautés.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<KeyValuePair<TvSeason, TvEpisode>> GetAllNouveautes()
		{
			return _nouveautes;
		}

		/// <summary>
		/// Permet de réinitialisé la liste des nouveautés.
		/// </summary>
		public void ResetNouveautes()
		{
			_nouveautes = new List<KeyValuePair<TvSeason, TvEpisode>>();
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Met à jour le ShowModel. Si plus d'épisode, plus de saison.
		/// Le ShowModel doit être supprimé.
		/// </summary>
		private void UpdateShowModel()
		{
			var hsowToDelete = _list.Where(x => x.ShowInformation.Count == 0
			                                    && x.TvEpisodes.Count == 0
			                                    && x.TvSeasons.Count == 0).ToList();

			lock (_objectToLock)
			{
				foreach (ShowModel showModel in hsowToDelete)
				{
					_list.Remove(showModel);
				}
			}
		}

	    private bool IsSameShow(string titleTmDb, string titleInformationShow)
	    {
		    string tempTmDb = titleTmDb.Replace("'", string.Empty);
		    return tempTmDb.ToUpper() == titleInformationShow.ToUpper();
	    }

		/// <summary>
		/// Permet l'ajout d'une nouveaute.
		/// </summary>
		/// <param name="season"></param>
		/// <param name="episode"></param>
		private void AddNouveaute(TvSeason season, TvEpisode episode)
		{
			_nouveautes.Add(new KeyValuePair<TvSeason, TvEpisode>(season, episode));
		}

		#endregion

		
    }
}

