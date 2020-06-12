namespace Widtop.Utility
{
    public class Sample
    {
        private int _size;
        private float? _min;
        private float? _max;
        private float _total;

        public float Min => _min ?? 0;
        public float Max => _max ?? 0;
        public float Avg => _total / _size;

        public void Add(float value)
        {
            _size++;
            _total += value;

            if (!_min.HasValue || value < _min)
            {
                _min = value;
            }

            if (!_max.HasValue || value > _max)
            {
                _max = value;
            }
        }

        public void Reset()
        {
            _size = 0;
            _min = null;
            _max = null;
            _total = 0;
        }
    }
}
