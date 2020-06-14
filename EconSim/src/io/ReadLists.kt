package io

import io.Read.Companion.MalformedItemException
import io.Read.Companion.NoSuchTypeException
import io.Read.Companion.UnexpectedCharacterException

inline fun <reified T> readList(name: String, type: String, tokenIterator: Iterator<String>): List<T> {
    val listOfAny = mutableListOf<Any>()
    while (tokenIterator.hasNext()) {
        val nextListElement: Any? = readNextListElement(name, type, tokenIterator)
        if (nextListElement == null) {
            return filterList(name, type, listOfAny)
        } else {
            listOfAny.add(nextListElement)
            if (tokenIterator.hasNext().not()) break
            when (tokenIterator.next()) {
                "]" -> return filterList(name, type, listOfAny)
                "," -> {}
                else -> throw MalformedItemException("list $name", "must use commas to separate list elements")
            }
        }
    }
    throw MalformedItemException("list $name", "has no closing ']'")
}

fun readNextListElement(listName: String, type: String, tokenIterator: Iterator<String>): Any? {
    if (tokenIterator.hasNext().not()) throw MalformedItemException("list $listName", "has no closing ']'")
    when (val token = tokenIterator.next()) {
        "=" -> throw UnexpectedCharacterException('=', "list $listName", "cannot use assignment in list")
        "}" -> throw UnexpectedCharacterException('}', "list $listName", "cannot close undeclared object in list")
        "[" -> throw UnexpectedCharacterException('[', "list $listName", "cannot declare list in list")
        "(" -> throw UnexpectedCharacterException('(', "list $listName", "cannot declare primitive in list")
        ")" -> throw UnexpectedCharacterException(')', "list $listName", "cannot declare primitive in list")
        "," -> throw UnexpectedCharacterException(',', "list $listName", "cannot have empty element in list")
        "{" -> {return 1}
        "]" -> return null
        else -> {
            return when (type) {
                "Int" -> readInt(token)
                "Float" -> readFloat(token)
                "String" -> token
                else -> throw NoSuchTypeException(type)
            }
        }
    }
}

inline fun <reified T> filterList(listName: String, type: String, listOfAny: List<Any>): List<T> {
    val listOfT = listOfAny.filterIsInstance<T>()
    if (listOfT.size == listOfAny.size) return listOfT
    else throw MalformedItemException("list $listName", "cannot contain elements not of type $type")
}


inline fun <reified T> readList2(name: String, type: String, tokenIterator: Iterator<String>): List<T> {
    val listOfAny = handleList2(name, type, tokenIterator)
    val listOfT = mutableListOf<T>()

    for (element in listOfAny) {
        if (element !is T) throw MalformedItemException("list $name", "cannot contain elements not of type $type")
        listOfT.add(element)
    }
    return listOfT.toList()
}

fun handleList2(name: String, type: String, tokenIterator: Iterator<String>): List<Any> {
    val listOfElements = mutableListOf<Any>()
    var currentElement: Any? = null
    while (tokenIterator.hasNext()) {
        when (val token = tokenIterator.next()) {
            "=" -> throw UnexpectedCharacterException('=', "list $name", "cannot use assignment in list")
            "}" -> throw UnexpectedCharacterException('}', "list $name", "cannot close undeclared object in list")
            "[" -> throw UnexpectedCharacterException('[', "list $name", "cannot declare list in list")
            "(" -> throw UnexpectedCharacterException('(', "list $name", "cannot declare primitive in list")
            ")" -> throw UnexpectedCharacterException(')', "list $name", "cannot declare primitive in list")

            "{" -> {
                if (currentElement != null) throw UnexpectedCharacterException('{', "list $name", "list elements must be separated with commas")
            }

            "," -> {
                if (currentElement == null) throw UnexpectedCharacterException(',', "list $name", "cannot have empty element in list")
                listOfElements.add(currentElement)
                currentElement = null
            }

            "]" -> {
                if (currentElement != null) listOfElements.add(currentElement)
                return listOfElements.toList()
            }

            else -> {
                if (currentElement != null) throw MalformedItemException("list $name", "must separate list items with commas")
                currentElement = handleListPrimitive2(type, token)
            }
        }
    }
    throw MalformedItemException("list $name", "has no closing ']'")
}

private fun handleListPrimitive2(type: String, token: String): Any {
    when (type) {
        "Int" -> return readInt(token)
        "Float" -> return readFloat(token)
        "String" -> return token
    }
    throw NoSuchTypeException(type)
}
