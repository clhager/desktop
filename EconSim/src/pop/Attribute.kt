package pop

import main.Statistics.Companion.memoizedInverseNormalCDF
import java.lang.IllegalArgumentException

class Attribute(
    val id: Int,
    private val tag: String,
    private val mean: Int,
    private val standardDeviation: Int
) {
    constructor(
        tag: String,
        mean: Int,
        standardDeviation: Int
    ) : this(nextAttributeId, tag, mean, standardDeviation)

    constructor(
        map: Map<String, Any>
    ) : this(
        map["id"] as? Int ?: nextAttributeId,
        map["tag"] as String,
        map["mean"] as Int,
        map["standardDeviation"] as Int
    )

    init {
        allAttributeIds.add(id)
        findNextValidAttributeId()
        if (allAttributes.containsKey(tag)) throw IllegalArgumentException("Cannot create Attribute with duplicate tag $tag")
        allAttributes[tag] = this
    }

    companion object {
        var nextAttributeId: Int = 1
        val allAttributeIds = mutableSetOf<Int>()
        val allAttributes = mutableMapOf<String, Attribute>()
    }

    fun getValue(popDistributionValue: Int): Int =
        (mean + memoizedInverseNormalCDF(popDistributionValue) * standardDeviation).toInt()

    override fun toString(): String {
        return "Attribute{" +
                "id=Int($id)," +
                "tag=String($tag)," +
                "mean=Int($mean)," +
                "standardDeviation=Int($standardDeviation)" +
                "}"
    }

    private fun findNextValidAttributeId()  {
        while (allAttributeIds.contains(nextAttributeId)) nextAttributeId++
    }
}