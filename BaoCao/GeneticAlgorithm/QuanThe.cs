using System.Collections.Generic;
using System.Linq;

namespace BaoCao.GeneticAlgorithm
{
    public class QuanThe
    {
        private List<HashSet<CaThe>> _caTheList;

        public int SoTang => _caTheList.Count;
        public int Count { get; private set; }


        /// <summary>
        /// O(N)
        /// </summary>
        /// <param name="phanTang"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public CaThe this[int phanTang, int index] => _caTheList[phanTang].ElementAt(index);


        /// <summary>
        /// O(_graph.M)
        /// </summary>
        /// <param name="soTang"></param>
        public QuanThe(int soTang)
        {
            Count = 0;
            _caTheList = new List<HashSet<CaThe>>();
            for (int i = 0; i < soTang; i++)
            {
                _caTheList.Add(new HashSet<CaThe>(new CaTheComparer()));
            }
        }


        /// <summary>
        /// O(1)
        /// </summary>
        /// <param name="phanTang"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public bool Add(int phanTang, CaThe a)
        {
            if (_caTheList[phanTang].Add(a))
            {
                Count++;
                return true;
            }
            return false;
        }


        /// <summary>
        /// O(N)
        /// </summary>
        /// <param name="phanTang"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool Remove(int phanTang, int index)
        {
            if (_caTheList[phanTang].Remove(_caTheList[phanTang].ElementAt(index)))
            {
                Count--;
                return true;
            }
            return false;
        }


        /// <summary>
        /// O(N)
        /// </summary>
        /// <param name="phanTang"></param>
        public void RemovePhanTang(int phanTang)
        {
            Count -= this.SoLuongCaThe(phanTang);
            _caTheList[phanTang].Clear();
        }


        /// <summary>
        /// O(N)
        /// </summary>
        /// <param name="phanTang"></param>
        /// <param name="startIndex"></param>
        public void RemoveRange(int phanTang, int startIndex)
        {
            // O(N)
            var list = _caTheList[phanTang].ToList();

            // O(N)
            int deleteNumber = list.Count - startIndex;
            list.RemoveRange(startIndex, deleteNumber);
            Count -= deleteNumber;

            // O(N)
            _caTheList[phanTang] = list.ToHashSet(new CaTheComparer());
        }


        /// <summary>
        /// O(1)
        /// </summary>
        /// <param name="phanTang"></param>
        /// <returns></returns>
        public int SoLuongCaThe(int phanTang)
        {
            return _caTheList[phanTang].Count;
        }
    }
}