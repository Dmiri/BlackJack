using System;
using System.Security.Cryptography;

namespace ModuleTask
{
    [Serializable]
    class Bot : Gambler
    {
        /// <summary>
        /// The logic of behavior bots.
        /// </summary>
        /// <returns>true if bot take card and fals otherwise.</returns>
        public bool RunBotMeditate()
        {
            bool take = false;
            //Console.WriteLine("Will you take a card?");
            //Console.WriteLine("There must be a calculation of decision making by the bot, but it's just a random.");
            if (this.points < 12) return true;
            if (this.points == 20) return false;
            RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
            byte[] rand = new byte[1];

            var YesNo = rand[0] % 2;
            if (YesNo == 0) take = false;
            else take = true;
            return take;
        }
    }
}
