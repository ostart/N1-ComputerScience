using NUnit.Framework;
using N1_ComputerScience;

namespace Tests
{
    [TestFixture]
    public class TestsPerceptron
    {
        private Perceptron _neuron;

        [SetUp]
        public void SetUp()
        {
            var weigths = new decimal[10,10]
            {
                {1,1,1,1,1,1,1,1,1,1},
                {1,0,0,0,3,3,0,0,3,1},
                {1,0,0,3,0,0,3,3,0,1},
                {1,0,0,3,0,0,3,0,0,1},
                {1,0,3,0,0,3,0,3,0,1},
                {1,1,1,1,1,1,1,1,1,1},
                {1,3,0,3,0,0,0,0,3,1},
                {1,3,3,0,0,0,0,0,3,1},
                {1,3,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,1}
            };

            _neuron = new Perceptron(weigths, 35);
        }


        [TestCase("../../../PerceptronInput/positiveA_v1.txt")]
        [TestCase("../../../PerceptronInput/positiveA_v2.txt")]
        [TestCase("../../../PerceptronInput/positiveA_v3.txt")]
        [TestCase("../../../PerceptronInput/negativeB_v2.txt")]
        [TestCase("../../../PerceptronInput/negativeИ.txt")]
        public void TestPerceptron_PositiveInput_TrueResult(string filePath)
        {
            Assert.AreEqual(1, _neuron.CalculateOutput(filePath));
        }

        [TestCase("../../../PerceptronInput/negative6.txt")]
        [TestCase("../../../PerceptronInput/negativeB_v1.txt")]
        [TestCase("../../../PerceptronInput/negativeC.txt")]
        [TestCase("../../../PerceptronInput/negativeE.txt")]
        [TestCase("../../../PerceptronInput/negativeF.txt")]
        [TestCase("../../../PerceptronInput/negativeH.txt")]
        [TestCase("../../../PerceptronInput/negativeS.txt")]
        public void TestPerceptron_NegativeInput_FalseResult(string filePath)
        {
            Assert.AreEqual(0, _neuron.CalculateOutput(filePath));
        }

        [TestCase("../../../PerceptronInput/")]
        public void TestPerceptronEducation_DiversInputs_WeightsResult(string folderPath)
        {
            var neuron = new Perceptron(10, 10, 35);
            var resultWeight = neuron.Educate(folderPath, 100, out var counter);
            var ethalonWeigths = new decimal[10,10]
            {
                {-1,0,0,0,2,2,0,0,0,0},
                {-1,0,0,0,2,2,0,0,1,4},
                {-1,0,0,2,0,0,2,1,0,4},
                {-1,0,0,2,0,0,3,0,0,4},
                {-1,0,2,0,0,1,0,2,0,4},
                {-1,-1,1,1,2,2,2,2,0,0},
                {-1,2,0,1,0,0,0,0,2,2},
                {-1,2,1,0,0,0,0,0,2,2},
                {1,1,0,0,0,0,0,0,0,4},
                {2,-2,-2,-2,-2,-2,-2,-2,-2,4}
            };

            Assert.AreEqual(7, counter);
            Assert.AreEqual(ethalonWeigths.GetLength(0), resultWeight.GetLength(0));
            Assert.AreEqual(ethalonWeigths.GetLength(1), resultWeight.GetLength(1));
                        
            var flag = true;
            for (int i = 0; i < resultWeight.GetLength(0); i++)
                for (int j = 0; j < resultWeight.GetLength(1); j++)
                    if (resultWeight[i,j] != ethalonWeigths[i,j]) flag = false;
            
            Assert.IsTrue(flag);
        }
    }
}