package main

import io.Read
import io.Write
import pop.Pop

fun main(args: Array<String>) {
    val popArray: MutableList<Pop> = mutableListOf()

    val pop = Pop(1)

    Read.readFromFile("hello.txt")

    Write.writeToFile("hello.txt", Pop(1).toString())


    Statistics.inverseNormalCDF(0)
    val nums = mutableListOf<Double>()
    for (i in -50 until 51) {
        nums.add(Statistics.inverseNormalCDF(i))
    }

    println(nums.filter { (it >= -3) and (it < -2) }.size)
    println(nums.filter { (it >= -2) and (it < -1) }.size)
    println(nums.filter { (it >= -1) and (it < 0) }.size)
    println(nums.filter { (it == 0.0) }.size)
    println(nums.filter { (it > 0) and (it <= 1) }.size)
    println(nums.filter { (it > 1) and (it <= 2) }.size)
    println(nums.filter { (it > 2) and (it <= 3) }.size)

}