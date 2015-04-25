using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;

namespace EmguLeap
{
	public class MedianFilter1D<T>
	{
		// output - filtered input
		//std::vector<T> m_array;
		// histogram
		//std::map<T, int> m_histogram;

		//typedef typename std::map<T, int>::iterator iterator;
		// current median - position in tree
		//iterator m_median;

		private ArrayList Data;
		private Dictionary<int, T> Histogram;

		public MedianFilter1D(ArrayList data, int windowSize = 3)
		{
			WindowSize = windowSize;
			Data = data;
			Histogram = new Dictionary<int, T>();
		}

		public object this[int i]
		{
			get { return FilteredData[i]; }
		}

		public int Count { get { return FilteredData.Count; } }
		public bool IsEmpty { get { return FilteredData.Count == 0; } }

		public int WindowSize { get; private set; }

		public void Start(ArrayList array)
		{
			FilteredData.Clear();
		}

	void Execute(const std::vector<T>& v, bool enabled = true)
	{
		// clear output
		m_array.clear();
		// clear histogram
		m_histogram.erase(m_histogram.begin(), m_histogram.end());

		if(enabled)
		{
			// if filter is enabled - perform filtering
			FilterImpl(v);
		}
		else
		{
			// if filter is disabled - make simply copy of input
			m_array.insert(m_array.end(), v.begin(), v.end());
		}
	};

private:

	void Inc(const T& key)
	{
		std::map<T, int>::iterator it = m_histogram.find(key);
		if(it != m_histogram.end())
			it->second++;
		else
			m_histogram.insert(std::pair<T,int>(key, 1));
	};

	void Dec(const T& key)
	{
		std::map<T, int>::iterator it = m_histogram.find(key);
		if(it != m_histogram.end())
		{
			if(it->second == 1)
			{
				if(m_median != it)
					m_histogram.erase(it);
				else
					it->second = 0;
			}
			else
			{
				it->second--;
			}
		}
	};


		void FilterImpl(const std::vector<T>& obj)
		{
			T av, dv;
			int sum = 0, dl,  middle = m_windowsize/2+1;

			for (int j = -m_windowsize/2; j <= m_windowsize/2; j++ )
			{
				if ( j < 0 )
				{
					av = obj.front();
				}
				else if(j > obj.size()-1)
				{
					av = obj.back();
				}
		  		else
				{
					av = obj[j];
				}
				Inc(av);
			}


			for(m_median = m_histogram.begin(); m_median != m_histogram.end(); m_median++)
			{
				sum += m_median->second;
				if( sum >= middle )
					break;
			}

			m_array.push_back(m_median->first);
			dl = sum - m_median->second;

			int N = obj.size();
			for (int j = 1; j < N; j++)
			{
				int k = j - m_windowsize/2-1;
				dv = k < 0 ? obj.front() : obj[k];
				k = j + m_windowsize/2;
				av = k > (int)obj.size()-1 ? obj.back() : obj[k];
				if(av != dv)
				{
					Dec(dv);
					if(dv < m_median->first)
						dl--;
					Inc(av);
					if( av < m_median->first )
						dl++;
					if(dl >= middle)
					{
						while(dl >= middle)
						{
							m_median--;
							dl -= m_median->second;
						}
	   				}
					else
					{
						while (dl + m_median->second < middle)
						{
							dl += m_median->second;
							m_median++;
						}
					}
				}
				m_array.push_back(m_median->first);
			}
		};

};

}
