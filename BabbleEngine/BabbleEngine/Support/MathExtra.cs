using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BabbleEngine.Support
{
    /// <summary>
    /// A static class for all helper functions.
    /// </summary>
    public static class MathExtra
    {
        // This is the random generator.
        private static Random random = new Random();

        /// <summary>
        /// Returns a random float between 0.0 and 1.0.
        /// </summary>
        public static float RandomFloat()
        {
            return (float)random.NextDouble();
        }

        /// <summary>
        /// Returns a random number less than i.
        /// <param name="i">The upper bound.</param>
        /// </summary>
        public static int RandomInt(int i)
        {
            return random.Next(i);
        }

        /// <summary>
        /// Shuffles the given list randomly.
        /// </summary>
        public static void ShuffleList<T>(IList<T> list)
        {
            if (list.Count > 1)
            {
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    T tmp = list[i];
                    int randomIndex = random.Next(i + 1);

                    list[i] = list[randomIndex];
                    list[randomIndex] = tmp;
                }
            }
        }

        // ==============================
        // ======= VECTOR METHODS =======
        // ==============================

        /// <summary>
        /// Adds some length to a given vector.
        /// If the length is negative, and a length is subtracted larger than the current
        /// length of the vector, the vector is set to zero.
        /// </summary>
        /// <param name="vector">The vector to preform on.</param>
        /// <param name="x">The amount of units to subtract from the vector.</param>
        public static void AddToVectorLength(ref Vector2 vector, float x)
        {
            float length = vector.Length();
            if (length <= -x)
            {
                vector = Vector2.Zero;
            }
            else
            {
                vector *= (length + x) / length;
            }
        }

        /// <summary>
        /// Sets the length of a vector.
        /// </summary>
        public static void SetVectorLength(ref Vector2 vector, float x)
        {
            if (vector != Vector2.Zero)
            {
                vector.Normalize();
                vector *= x;
            }
        }

        /// <summary>
        /// Restricts the given vector to a maximum length.
        /// </summary>
        public static void RestrictVectorLength(ref Vector2 vector, float x)
        {
            if (vector.Length() > x)
                SetVectorLength(ref vector, x);
        }

        /// <summary>
        /// Returns if the two parallel vectors face the same direction or not.
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static bool CheckParallelVectorDirection(Vector2 v1, Vector2 v2)
        {
            return (Vector2.Dot(v1, v2) > 0);
        }

        /// <summary>
        /// Restricts a vector based on different components.
        /// </summary>
        public static void RestrictComponentLength(ref Vector2 vector, Vector2 xDirection, float x, float y)
        {
            Vector2 _xPart = MathExtra.GetProjectionVector(vector, xDirection);
            Vector2 _yPart = vector - _xPart;
            RestrictVectorLength(ref _xPart, x);
            RestrictVectorLength(ref _yPart, y);
            vector = _xPart + _yPart;
        }

        /// <summary>
        /// Returns a unit vector corresponding to the given angle.
        /// </summary>
        /// <param name="angle">An angle in radians.</param>
        /// <returns>A unit vector.</returns>
        public static Vector2 GetVectorFromAngle(float angle)
        {
            Vector2 a = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
            return a;
        }

        public static float GetAngleFromVector(Vector2 v)
        {
            return (float)Math.Atan2(v.Y, v.X);
        }

        /// <summary>
        /// Returns a random unit vector.
        /// </summary>
        /// <returns>A random unit vector.</returns>
        public static Vector2 GetRandomUnitVector()
        {
            return GetVectorFromAngle(RandomFloat() * MathHelper.Pi * 2);
        }

        /// <summary>
        /// Finds the vector perpendicular to the given vector with the same length.
        /// In particular, the vector rotated 90 degrees clockwise.
        /// </summary>
        /// <param name="v">A vector to use.</param>
        /// <returns>A perpendicular vector.</returns>
        public static Vector2 GetPerpendicularVector(Vector2 v)
        {
            return new Vector2(v.Y, -v.X);
        }

        /// <summary>
        /// Gets the projection vector of one onto another.
        /// </summary>
        /// <param name="vector">The vector to find the projection of.</param>
        /// <param name="projOnTo">The projection plane.</param>
        /// <returns>A projection of a vector.</returns>
        public static Vector2 GetProjectionVector(Vector2 vector, Vector2 projOnTo)
        {
            return projOnTo * Vector2.Dot(vector, projOnTo) / projOnTo.LengthSquared();
        }


        /// <summary>
        /// Finds the distance between two angles.
        /// </summary>
        public static float DistanceBetweenAngle(float angleA, float angleB)
        {
            angleA = MathHelper.WrapAngle(angleA);
            angleB = MathHelper.WrapAngle(angleB);

            float biggerAngle = angleA >= angleB ? angleA : angleB;
            float smallerAngle = angleA >= angleB ? angleB : angleA;

            float innerAngle = biggerAngle - smallerAngle;
            float outerAngle = (smallerAngle - (-MathHelper.Pi)) + ((MathHelper.Pi - biggerAngle));

            return innerAngle > outerAngle ? outerAngle : innerAngle;

        }


        /// <summary>
        /// Checks if the angle "middle" is between the acute angle between "angleA" and "angleB."
        /// </summary>
        public static bool BetweenAngle(float angleA, float angleB, float middle)
        {
            angleA = MathHelper.WrapAngle(angleA);
            angleB = MathHelper.WrapAngle(angleB);
            middle = MathHelper.WrapAngle(middle);

            float biggerAngle = angleA >= angleB ? angleA : angleB;
            float smallerAngle = angleA >= angleB ? angleB : angleA;

            float innerAngle = biggerAngle - smallerAngle;
            float outerAngle = (smallerAngle - (-MathHelper.Pi)) + ((MathHelper.Pi - biggerAngle));

            if (innerAngle < outerAngle)
                return (biggerAngle > middle && middle > smallerAngle);
            else
                return (middle > biggerAngle || middle < smallerAngle);
        }
    }
}
