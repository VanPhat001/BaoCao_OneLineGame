using System.Collections.Generic;
using System.Linq;

namespace BaoCao.GeneticAlgorithm
{
    public class Population
    {
        private List<HashSet<Individual>> _individualList;

        public int Floors => _individualList.Count;
        public int Count { get; private set; }


        /// <summary>
        /// lấy cá thể ở phân tầng floor tại chỉ số index. O(N)
        /// </summary>
        /// <param name="floor"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public Individual this[int floor, int index] => _individualList[floor].ElementAt(index);


        /// <summary>
        /// khởi tạo quần thể có floors phân tầng. O(_graph.M)
        /// </summary>
        /// <param name="floors"></param>
        public Population(int floors)
        {
            Count = 0;
            _individualList = new List<HashSet<Individual>>();
            for (int i = 0; i < floors; i++)
            {
                _individualList.Add(new HashSet<Individual>(new CaTheComparer()));
            }
        }


        /// <summary>
        /// thêm cá thể a vào phân tầng floor O(1)
        /// </summary>
        /// <param name="floor"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public bool Add(int floor, Individual a)
        {
            if (_individualList[floor].Add(a))
            {
                Count++;
                return true;
            }
            return false;
        }


        /// <summary>
        /// xóa cá thể tại chỉ số index trong phân tầng floor. O(N)
        /// </summary>
        /// <param name="floor"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool Remove(int floor, int index)
        {
            if (_individualList[floor].Remove(_individualList[floor].ElementAt(index)))
            {
                Count--;
                return true;
            }
            return false;
        }


        /// <summary>
        /// xóa tất cả cá thể trong phân tầng floor. O(N)
        /// </summary>
        /// <param name="floor"></param>
        public void RemoveFloor(int floor)
        {
            Count -= this.CountIndividual(floor);
            _individualList[floor].Clear();
        }


        /// <summary>
        /// xóa các cá thể trong phân tầng floor, bắt đầu tại chỉ số startIndex đến hết. O(N)
        /// </summary>
        /// <param name="floor"></param>
        /// <param name="startIndex"></param>
        public void RemoveRange(int floor, int startIndex)
        {
            // O(N)
            var list = _individualList[floor].ToList();

            // O(N)
            int deleteNumber = list.Count - startIndex;
            list.RemoveRange(startIndex, deleteNumber);
            Count -= deleteNumber;

            // O(N)
            _individualList[floor] = list.ToHashSet(new CaTheComparer());
        }


        /// <summary>
        /// đếm số lượng cá thể có trong phân tầng floor. O(1)
        /// </summary>
        /// <param name="floor"></param>
        /// <returns></returns>
        public int CountIndividual(int floor)
        {
            return _individualList[floor].Count;
        }
    }
}