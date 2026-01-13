using SoccerLinkPlayerSideApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoccerLinkPlayerSideApp.Services
{
    public class UserSessionService
    {
        private Zawodnik? _currentUser;

        public Zawodnik? CurrentUser => _currentUser;
        public bool IsLoggedIn => _currentUser != null;

        public void SetUser(Zawodnik zawodnik)
        {
            _currentUser = zawodnik;
        }

        public void ClearUser()
        {
            _currentUser = null;
        }
    }
}
