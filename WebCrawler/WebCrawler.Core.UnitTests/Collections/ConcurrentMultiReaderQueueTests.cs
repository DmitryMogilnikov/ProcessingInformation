using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler.Core.Collections;

namespace WebCrawler.Core.UnitTests.Collections
{
    public class ConcurrentMultiReaderQueueTests
    {
        [Fact]
        public void TestTryDequeue_ThrowsArgumentOutOfRangeException() // оставляем
        {
            
            ConcurrentMultiReaderQueue<int> concurrentQueue = new ConcurrentMultiReaderQueue<int>(5, null); 
            int value;
            
            Action act = () => concurrentQueue.TryDequeue(10, out value);
           
         
            Assert.Throws<ArgumentOutOfRangeException>(act);
        }



        [Fact]
        public void TestTryDequeue_WhenEmpty() // оставляем
        {

            ConcurrentMultiReaderQueue<int> concurrentQueue = new ConcurrentMultiReaderQueue<int>(5, null);
            int value = 1;

            bool act = concurrentQueue.TryDequeue(3, out value);


            Assert.False(act);
        }
        
        
        
        [Fact]
        public void TestIsBucketEmpty() // Test"название метода" - 1 строка 
        {

            ConcurrentMultiReaderQueue<int> concurrentQueue = new ConcurrentMultiReaderQueue<int>(5, EqualityComparer<int>.Default); /* просто название класса (добавляем скобочки, если они есть в названии класса)
                                                                                                             * название переменной (любое, но понятное) 
                                                                                                             * =
                                                                                                             * new
                                                                                                             * просто название класса (добавляем скобочки, если они есть в названии класса)
                                                                                                             * (аргументы конструктора см. в выпадающей подсказке)    если в ошибке написано, что не удавется преобразовать в WebCrawler..., то нужно создать ЭТО ЧТО-ТО с помощью конструктора (класс создать с переменной, а потом создать второй класс, используя в кач-ве аргумента переменную с первым классом)
                                                                                                             * ;
                                                                                                             */
            
          

            bool act = concurrentQueue.IsBucketEmpty(3);        /* 1. Action нужен, если тест проверяет на ошибки (если дальше исп. Assert.Throws) --- методы, которые ищут ошибки - это те, где есть слово throw
                                                                 * если с Action то после него 1.2 = () => 1.4
                                                                   * 1.1. (во всех др. случаях кроме Action) тип, который возвращает метод
                                                                   * 1.2. имя переменной (новая, в которую будем записывать результат вызова метода) 
                                                                   * 1.3 =
                                                                   * 1.4 название переменной (любое, но понятное) --- это из предыдущей (2) строки 
                                                                   * .
                                                                   * имя тестируемого метда (как в 1 строке)
                                                                   * (аргументы метода, который тестируем) 
                                                                   * ;
                                                                   *
                                                                   * если void (перед именем метода в классе), то не нужны строчки: 1.1, 1.2, 1.3, а все остальное то же самое 
                                                                   */



            Assert.True(act);             /*  Assert
                                           *  .
                                           *  1.1 если тест проверяет ошибку, то Throws<тип ошибки>(1.2 --- переменная Action) 
                                           *  1.2 Equal(то что ожидаем что метод вернет, то что реально вернул - это записано в переменной 1.2)
                                           *  1.3 если void, то наверное метод что-то меняет, тогда проверяем что поменялось так, как и ожидалось --- Equal(ожидаемое измененное значение, реально измененное значение)
                                           *  ;
                                           */  
        }

        [Fact]
        public void TestIsEmpty_WhenEmpty()
        {
            ConcurrentMultiReaderQueue<int> concurrentQueue = new ConcurrentMultiReaderQueue<int>(5, EqualityComparer<int>.Default);

            bool act = concurrentQueue.IsEmpty();

            Assert.True(act);
        }

        [Fact]
        public void TestIsEmpty_WhenNotEmpty()
        {
            ConcurrentMultiReaderQueue<int> concurrentQueue = new ConcurrentMultiReaderQueue<int>(5, EqualityComparer<int>.Default);
            concurrentQueue.TryEnqueue(1);

            bool act = concurrentQueue.IsEmpty();

            Assert.False(act);
        }

        [Fact]
        public void TestTryDequeue_WhenNotEmpty()
        {
            ConcurrentMultiReaderQueue<int> concurrentQueue = new ConcurrentMultiReaderQueue<int>(1, EqualityComparer<int>.Default);
            concurrentQueue.TryEnqueue(1);

            int value;
            bool act = concurrentQueue.TryDequeue(0, out value);

            Assert.True(act);
            Assert.Equal(1, value);
        }
    }


}


