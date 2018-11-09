﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace MIConvexHull
{
    /// <summary>
    /// Factory class for computing convex hulls.
    /// </summary>
    public static class ConvexHull
    {
        /// <summary>
        /// Creates a convex hull of the input data.
        /// </summary>
        /// <typeparam name="TVertex">The type of the t vertex.</typeparam>
        /// <typeparam name="TFace">The type of the t face.</typeparam>
        /// <param name="data">The data.</param>
        /// <param name="PlaneDistanceTolerance">The plane distance tolerance (default is 1e-10). If too high, points 
        /// will be missed. If too low, the algorithm may break. Only adjust if you notice problems.</param>
        /// <returns>
        /// ConvexHull&lt;TVertex, TFace&gt;.
        /// </returns>
        public static ConvexHullCreationResult<TVertex, TFace> Create<TVertex, TFace>(IList<TVertex> data,
                                                                        double PlaneDistanceTolerance =
                                                                            Constants.DefaultPlaneDistanceTolerance)
            where TVertex : IVertex
            where TFace : ConvexFace<TVertex, TFace>, new()
        {
            return ConvexHull<TVertex, TFace>.Create(data, PlaneDistanceTolerance);
        }

        /// <summary>
        /// Creates a convex hull of the input data.
        /// </summary>
        /// <typeparam name="TVertex">The type of the t vertex.</typeparam>
        /// <param name="data">The data.</param>
        /// <param name="PlaneDistanceTolerance">The plane distance tolerance (default is 1e-10). If too high, points 
        /// will be missed. If too low, the algorithm may break. Only adjust if you notice problems.</param>
        /// <returns>
        /// ConvexHull&lt;TVertex, DefaultConvexFace&lt;TVertex&gt;&gt;.
        /// </returns>
        public static ConvexHullCreationResult<TVertex, DefaultConvexFace<TVertex>> Create<TVertex>(IList<TVertex> data,
                                                                                      double PlaneDistanceTolerance =
                                                                                          Constants.DefaultPlaneDistanceTolerance)
            where TVertex : IVertex
        {
            return ConvexHull<TVertex, DefaultConvexFace<TVertex>>.Create(data, PlaneDistanceTolerance);
        }

        /// <summary>
        /// Creates a convex hull of the input data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="PlaneDistanceTolerance">The plane distance tolerance (default is 1e-10). If too high, points 
        /// will be missed. If too low, the algorithm may break. Only adjust if you notice problems.</param>
        /// <returns>
        /// ConvexHull&lt;DefaultVertex, DefaultConvexFace&lt;DefaultVertex&gt;&gt;.
        /// </returns>
        public static ConvexHullCreationResult<DefaultVertex, DefaultConvexFace<DefaultVertex>> Create(IList<double[]> data,
                                                                                         double PlaneDistanceTolerance =
                                                                                             Constants.DefaultPlaneDistanceTolerance)
        {
            var points = data.Select(p => new DefaultVertex {Position = p})
                             .ToList();
            return ConvexHull<DefaultVertex, DefaultConvexFace<DefaultVertex>>.Create(points, PlaneDistanceTolerance);
        }
    }

    /// <summary>
    /// Representation of a convex hull.
    /// </summary>
    /// <typeparam name="TVertex">The type of the t vertex.</typeparam>
    /// <typeparam name="TFace">The type of the t face.</typeparam>
    public class ConvexHull<TVertex, TFace> where TVertex : IVertex
                                            where TFace : ConvexFace<TVertex, TFace>, new()
    {
        /// <summary>
        /// Can only be created using a factory method.
        /// </summary>
        internal ConvexHull()
        {
        }

        /// <summary>
        /// Points of the convex hull.
        /// </summary>
        /// <value>The points.</value>
        public IEnumerable<TVertex> Points { get; internal set; }

        /// <summary>
        /// Faces of the convex hull.
        /// </summary>
        /// <value>The faces.</value>
        public IEnumerable<TFace> Faces { get; internal set; }

        /// <summary>
        /// Creates the convex hull.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="PlaneDistanceTolerance">The plane distance tolerance.</param>
        /// <returns>
        /// ConvexHullCreationResult&lt;TVertex, TFace&gt;.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">The supplied data is null.</exception>
        /// <exception cref="ArgumentNullException">data</exception>
        public static ConvexHullCreationResult<TVertex, TFace> Create(IList<TVertex> data, double PlaneDistanceTolerance)
        {
            if (data == null)
            {
                throw new ArgumentNullException("The supplied data is null.");
            }

            try
            {
                var convexHull = ConvexHullAlgorithm.GetConvexHull<TVertex, TFace>(data, PlaneDistanceTolerance);
                return new ConvexHullCreationResult<TVertex, TFace>(convexHull,ConvexHullCreationResultOutcome.Success);
            }
            catch (ConvexHullGenerationException e)
            {
                return new ConvexHullCreationResult<TVertex, TFace>(null, e.Error, e.ErrorMessage);
            }
            catch (Exception e)
            {
                return new ConvexHullCreationResult<TVertex, TFace>(null, ConvexHullCreationResultOutcome.UnknownError, e.Message);
            }

        }
    }

    public class ConvexHullCreationResult<TVertex, TFace> where TVertex : IVertex
                                                          where TFace : ConvexFace<TVertex, TFace>, new()
    {
        public ConvexHullCreationResult(ConvexHull<TVertex, TFace> result, ConvexHullCreationResultOutcome outcome, string errorMessage="")
        {
            Result  = result;
            Outcome = outcome;
            ErrorMessage = errorMessage;
        }

        //this could be null
        public ConvexHull<TVertex, TFace> Result { get; }

        public ConvexHullCreationResultOutcome Outcome { get; }
        public string ErrorMessage { get; }
    }
}