package io

import io.Read.Companion.FileCastException
import io.Read.Companion.MalformedItemException

fun readIntPrimitive(input: String, tokenIterator: Iterator<String>): Int = readInt(readPrimitive("Int", tokenIterator))
fun readFloatPrimitive(input: String, tokenIterator: Iterator<String>): Float = readFloat(readPrimitive("Float", tokenIterator))
fun readStringPrimitive(input: String, tokenIterator: Iterator<String>): String = readPrimitive("String", tokenIterator)

fun readInt(input: String): Int = input.toIntOrNull() ?: throw FileCastException(input, "Int")
fun readFloat(input: String): Float = input.toFloatOrNull() ?: throw FileCastException(input, "Float")

private fun readPrimitive(type: String, tokenIterator: Iterator<String>): String {
    if (tokenIterator.hasNext().not()) throw MalformedItemException(type, "must have a value")
    val primitiveValue = tokenIterator.next()
    if (tokenIterator.hasNext().not() or (tokenIterator.next() != ")")) throw MalformedItemException(type, "must be closed with ')'")
    return primitiveValue
}