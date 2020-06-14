package main

import org.apache.commons.math3.distribution.NormalDistribution
import java.util.concurrent.ConcurrentHashMap
import kotlin.math.pow

class Statistics {
    companion object {
        const val normalDistributionDivisorRange = 101
        private const val adjustedNormalDistributionDivisorRange: Double = 100.27
        private const val decimalPrecision = 2

        fun memoizedInverseNormalCDF(adjustedZScore: Int): Double = { x: Int -> inverseNormalCDF(x) }.memoize()(adjustedZScore)

        fun inverseNormalCDF(adjustedZScore: Int): Double {
            val inverseCDFInput =  0.5 + (adjustedZScore.toDouble() / adjustedNormalDistributionDivisorRange)
            return NormalDistribution().inverseCumulativeProbability(inverseCDFInput).round(decimalPrecision)
        }

        private fun ((Int) -> Double).memoize(): (Int) -> Double {
            val memory = ConcurrentHashMap<Int, Double>()
            return { input ->
                memory[input] ?: this(input).apply {
                    memory[input] = this
                }
            }
        }

        private fun Double.round(precision: Int): Double = kotlin.math.round(this * 10.0.pow(precision)) / 10.0.pow(precision)
    }
}