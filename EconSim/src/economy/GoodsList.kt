package economy

class GoodsList {
    companion object {
        fun getGoodsList(): Map<Int, Good> {
            val goodsList = GoodsList()
            return mapOf(
                goodsList.createGoodPair("Food"),
                goodsList.createGoodPair("Wood"),
                goodsList.createGoodPair("Stone")
            )
        }
    }

    private var numberOfGoods = 0

    private fun createGoodPair(description: String): Pair<Int, Good> {
        val good = createGood(description)
        return Pair(good.id, good)
    }

    private fun createGood(description: String): Good {
        val newGood = Good(numberOfGoods + 1, description)
        numberOfGoods++
        return newGood
    }
}