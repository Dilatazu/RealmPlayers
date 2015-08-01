using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VF_RealmPlayersDatabase.PlayerData.PlayerHistoryUtility
{
    public class AddToHistory
    {
        public static void RunGeneric_T<T_HistoryItem>(List<T_HistoryItem> _HistoryArray, T_HistoryItem _NewHistoryItem, Func<T_HistoryItem, T_HistoryItem, bool> _IsSame, Func<T_HistoryItem, T_HistoryItem, bool> _Time1BiggerThan2, Func<T_HistoryItem, T_HistoryItem, bool> _CopyUploader2To1)
        {
            if (_HistoryArray.Count == 0)
            {
                //Finns inget i arrayen, spara ner det senaste
                _HistoryArray.Add(_NewHistoryItem);
                return;
            }
            for (int i = _HistoryArray.Count - 1; i >= 0; --i)
            {
                var currData = _HistoryArray[i];
                if (_Time1BiggerThan2(currData, _NewHistoryItem) == false)
                {//If _NewHistoryItem.Date >= currData.Date
                    if (_IsSame(currData, _NewHistoryItem) == true)
                        return;//Datan existerar redan och är tidigare nersparad
                    else
                    {
                        if (i + 1 < _HistoryArray.Count)
                        {
                            if (_IsSame(_HistoryArray[i + 1], _NewHistoryItem) == true)
                            {
                                //Datan existerar redan men är senare nersparad, ersätt gamla nersparningsdatumet
                                currData = _HistoryArray[i + 1];
                                _CopyUploader2To1(currData, _NewHistoryItem);
                                _HistoryArray[i + 1] = currData;
                            }
                            else
                            {
                                //Datan existerar inte, nersparningsdatumet är inte det senaste i arrayen
                                _HistoryArray.Insert(i + 1, _NewHistoryItem);
                            }
                        }
                        else
                        {
                            //Datan existerar inte, nersparningsdatumet är det senaste i arrayen
                            _HistoryArray.Add(_NewHistoryItem);
                        }
                        return;
                    }
                }
                //else
                //    break;//Eftersom arrayen alltid ska vara sorterad så kan vi breaka här
            }
            //Datan existerar inte, nersparningsdatumet är tidigare än vad som existerar
            if (_Time1BiggerThan2(_NewHistoryItem, _HistoryArray[0]) == false && _IsSame(_HistoryArray[0], _NewHistoryItem) == false)
                _HistoryArray.Insert(0, _NewHistoryItem);
        }
        public static void RunGeneric(List<CharacterDataHistoryItem> _HistoryArray, CharacterData _Data, UploadID _Uploader)
        {
            RunGeneric_T(_HistoryArray, new CharacterDataHistoryItem(_Data, _Uploader), CharacterDataHistoryItem.IsSame, CharacterDataHistoryItem.Time1BiggerThan2, CharacterDataHistoryItem.CopyUploader2To1);
        }
        public static void RunGeneric(List<GuildDataHistoryItem> _HistoryArray, GuildData _Data, UploadID _Uploader)
        {
            RunGeneric_T(_HistoryArray, new GuildDataHistoryItem(_Data, _Uploader), GuildDataHistoryItem.IsSame, GuildDataHistoryItem.Time1BiggerThan2, GuildDataHistoryItem.CopyUploader2To1);
        }
        public static void RunGeneric(List<GearDataHistoryItem> _HistoryArray, GearData _Data, UploadID _Uploader)
        {
            RunGeneric_T(_HistoryArray, new GearDataHistoryItem(_Data, _Uploader), GearDataHistoryItem.IsSame, GearDataHistoryItem.Time1BiggerThan2, GearDataHistoryItem.CopyUploader2To1);
        }
        public static void RunGeneric(List<HonorDataHistoryItem> _HistoryArray, HonorData _Data, UploadID _Uploader)
        {
            RunGeneric_T(_HistoryArray, new HonorDataHistoryItem(_Data, _Uploader), HonorDataHistoryItem.IsSame, HonorDataHistoryItem.Time1BiggerThan2, HonorDataHistoryItem.CopyUploader2To1);
        }
        public static void RunGeneric(List<ArenaDataHistoryItem> _HistoryArray, ArenaData _Data, UploadID _Uploader)
        {
            RunGeneric_T(_HistoryArray, new ArenaDataHistoryItem(_Data, _Uploader), ArenaDataHistoryItem.IsSame, ArenaDataHistoryItem.Time1BiggerThan2, ArenaDataHistoryItem.CopyUploader2To1);
        }
        public static void RunGeneric(List<TalentsDataHistoryItem> _HistoryArray, string _Data, UploadID _Uploader)
        {
            RunGeneric_T(_HistoryArray, new TalentsDataHistoryItem(_Data, _Uploader), TalentsDataHistoryItem.IsSame, TalentsDataHistoryItem.Time1BiggerThan2, TalentsDataHistoryItem.CopyUploader2To1);
        }
    }
}
