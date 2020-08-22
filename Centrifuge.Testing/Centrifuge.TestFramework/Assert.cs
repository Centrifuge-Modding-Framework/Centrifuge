using System;

namespace Centrifuge.TestFramework
{
    public static class Assert
    {
        public static void NotZero(int i)
        {
            if (i == 0)
                throw new TestException("The number was zero.");
        }

        public static void Zero(int i)
        {
            if (i != 0)
                throw new TestException("The number was not zero.");
        }

        public static void IsNull(object o)
        {
            if (o != null)
                throw new TestException("The object is not null.");
        }

        public static void IsNotNull(object o)
        {
            if (o == null)
                throw new TestException("The object is null.");
        }

        public static void AreEqual(object a, object b)
        {
            if (!a.Equals(b))
                throw new TestException("The objects are not equal.");
        }

        public static void AreNotEqual(object a, object b)
        {
            if (a.Equals(b))
                throw new TestException("The objects are equal.");
        }

        public static void AreSame(object a, object b)
        {
            if (a != b)
                throw new TestException("The objects are not the same.");
        }

        public static void False(bool condition)
        {
            if (condition)
                throw new TestException("The condition was not met.");
        }

        public static void True(bool condition)
        {
            if (!condition)
                throw new TestException("The condition was not met.");
        }

        public static void DoesNotThrow(Action a)
        {
            var passed = true;

            try
            {
                a?.Invoke();
            }
            catch
            {
                passed = false;
            }

            if (!passed)
                throw new TestException("The action threw while it wasn't supposed to.");
        }

        public static void Throws(Action a)
        {
            var passed = false;
            try
            {
                a?.Invoke();
            }
            catch
            {
                passed = true;
            }

            if (!passed)
                throw new TestException("The action did not throw.");
        }

        public static void Throws<T>(Action a) where T : Exception
        {
            var passed = false;
            var exceptionType = "None";

            try
            {
                a?.Invoke();
            }
            catch (Exception e)
            {
                exceptionType = e.GetType().Name;

                if (e is T) passed = true;
            }

            if (!passed)
                throw new TestException($"Failed to observe '{typeof(T).Name}'. Instead observed '{exceptionType}'.");
        }
    }
}