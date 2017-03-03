using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Game
{
    class Highscore
    {
        public int _score { get; private set; }
        public char _letter1 { get; set; }
        public char _letter2 { get; set; }
        public char _letter3 { get; set; }
        public float _timer { get; private set; }

        public Highscore(int score, float timer, char initialNameLetters)
        {
            _score = score;
            _timer = timer;
            _letter1 = initialNameLetters;
            _letter2 = initialNameLetters;
            _letter3 = initialNameLetters;
        }

        public string GetTimerFormat()
        {
            string ret = "";

            int min = (int)_timer / 60; // calculate the minutes
            int sec = (int)_timer % 60; // calculate the seconds

            ret = min < 10 ? "0" + min : min.ToString();
            ret += ":";
            ret += sec < 10 ? "0" + sec : sec.ToString();

            return ret;
        }

        public static bool operator <(Highscore h1, Highscore h2)
        {
            return h1._score < h2._score;
        }

        public static bool operator >(Highscore h1, Highscore h2)
        {
            return h1._score > h2._score;
        }
    }
}
