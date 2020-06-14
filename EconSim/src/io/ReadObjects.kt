package io

import io.Read.Companion.UnexpectedCharacterException
import io.Read.Companion.MalformedItemException
import io.Read.Companion.NoSuchTypeException
import main.World
import pop.Attribute
import pop.Pop
import java.lang.Exception

fun readWorld(name: String, map: Map<String, Any>): World {
    return World(
        (map["pops"] as? List<Pop>) ?: throw Exception
    )
}

fun handleObject(name: String, type: String, tokenIterator: Iterator<String>): Any {
    when(type) {
        "Attribute" -> return readObject<Attribute>(name, type, tokenIterator)
        "Pop" -> return readObject<Pop>(name, type, tokenIterator)
        "World" -> return readObject<World>(name, type, tokenIterator)
    }
    throw NoSuchTypeException(type)
}

fun returnObject(name: String, type: String, map: Map<String, Any>): Any {
    when(type) {
        "World" -> return readWorld(name, map)
    }
    throw NoSuchTypeException(type)
}

inline fun <reified T> readObject(name: String, type: String, tokenIterator: Iterator<String>): T  {
    val map = mutableMapOf<String, Any>()
    var beforeAssignment = true
    var variableName: String? = null
    var variableType: String? = null
    var assignmentCompleted = false

    while (tokenIterator.hasNext()) {
        when (val token = tokenIterator.next()) {
            "]" -> throw UnexpectedCharacterException(']', "object $name", "cannot close list without list declaration")
            ")" -> throw UnexpectedCharacterException(')', "object $name", "cannot close primitive without primitive declaration")

            "=" -> {
                if (beforeAssignment.not()) throw UnexpectedCharacterException('=', "object $name", "duplicate character")
                if (variableName == null) throw UnexpectedCharacterException('=', "object $name", "variable names cannot be empty")
                beforeAssignment = false
            }

            "{" -> {
                if (beforeAssignment) throw UnexpectedCharacterException('{', "object $name", "cannot declare object without an assignment")
                if (variableName == null) throw UnexpectedCharacterException('{', "object $name", "cannot declare object without a variable name")
                if (variableType == null) throw UnexpectedCharacterException('{', "object $name", "cannot declare object $variableName without a type")
                map[variableName] = handleObject(variableName, variableType, tokenIterator)
                assignmentCompleted = true
            }

            "}" -> {
                if (assignmentCompleted.not()) {
                    if (variableName != null) throw UnexpectedCharacterException('}', "object $name", "cannot close object during variable declaration")
                }
                return objectHolder
            }

        }
    }
    throw MalformedItemException("object $name", "has no closing '}'")
}

private fun handleObject2(name: String, type: String, tokenIterator: Iterator<String>): Read.Companion.ObjectHolder {
    val objectHolder = Read.Companion.ObjectHolder(type)
    var beforeAssignment = true
    var variableName: String? = null
    var variableType: String? = null
    var assignmentCompleted = false

    while (tokenIterator.hasNext()) {
        when (val token = tokenIterator.next()) {
            "]" -> throw UnexpectedCharacterException(']', "object $name", "cannot close list without list declaration")
            ")" -> throw UnexpectedCharacterException(')', "object $name", "cannot close primitive without primitive declaration")

            "=" -> {
                if (variableName == null) throw UnexpectedCharacterException('=', "object $name", "variable names cannot be empty")
                if (beforeAssignment.not()) throw UnexpectedCharacterException('=', "object $name", "duplicate character")
                beforeAssignment = false
            }

            "{" -> {
                if (variableName == null) throw UnexpectedCharacterException('{', "object $name", "cannot declare object without a variable name")
                if (beforeAssignment) throw UnexpectedCharacterException('{', "object $name", "cannot declare object without an assignment")
                if (variableType == null) throw UnexpectedCharacterException('{', "object $name", "cannot declare object $variableName without a type")
                objectHolder.map[variableName] = handleObject(variableName, variableType, tokenIterator)
                assignmentCompleted = true
            }

            "}" -> {
                if (assignmentCompleted.not()) {
                    if (variableName != null) throw UnexpectedCharacterException('}', "object $name", "cannot close object during variable declaration")
                }
                return objectHolder
            }

            "[" -> {
                if (variableName == null) throw UnexpectedCharacterException('[', "object $name", "cannot declare list without a variable name")
                if (beforeAssignment) throw UnexpectedCharacterException('[', "object $name", "cannot declare list without an assignment")
                if (variableType == null) throw UnexpectedCharacterException('[', "object $name", "cannot declare list $variableName without a type")
                objectHolder.map[variableName] = Read.handleList(variableName, variableType, tokenIterator)
                assignmentCompleted = true
            }

            "(" -> {
                if (variableName == null) throw UnexpectedCharacterException('(', "object $name", "cannot declare primitive without a variable name")
                if (beforeAssignment) throw UnexpectedCharacterException('(', "object $name", "cannot declare primitive without an assignment")
                if (variableType == null) throw UnexpectedCharacterException('(', "object $name", "cannot declare primitive $variableName without a type")
                objectHolder.map[variableName] =
                    Read.handlePrimitive(variableName, variableType, tokenIterator)
                assignmentCompleted = true
            }

            "," -> {
                if (assignmentCompleted.not()) throw UnexpectedCharacterException(',', "object $name", "cannot have an empty variable")
                variableName = null
                variableType = null
                assignmentCompleted = false
                beforeAssignment = true
            }

            else -> {
                if (beforeAssignment) {
                    if (variableName != null) throw MalformedItemException("object $name", "cannot have variable names with more than one word")
                    else variableName = token
                } else {
                    if (variableType != null) throw MalformedItemException("object $name", "cannot have variable types with more than one word")
                    else variableType = token
                }
            }
        }
    }
    throw MalformedItemException("object $name", "has no closing '}'")
}