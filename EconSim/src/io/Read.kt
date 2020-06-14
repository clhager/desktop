package io

import main.World
import pop.Pop
import java.io.File
import java.lang.Exception
import java.lang.reflect.Executable
import kotlin.reflect.full.createInstance
import kotlin.reflect.full.primaryConstructor

class Read {
    companion object {
        private val tokenDividers = mutableListOf<Char>('=', '{', '}', '[', ']', '(', ')', ',')
        private val charactersAllowed = mutableListOf<Char>().apply {
            addAll('a'..'z')
            addAll('A'..'Z')
            addAll('0'..'9')
            addAll(tokenDividers)
        }

        fun readFromFile(path: String): World {
            val file = File(path)
            if (file.exists().not() or file.isFile.not()) throw IllegalArgumentException("File $path does not exist")

            val fileText = File(path).readText()
            val filteredText = fileText.filter { it in charactersAllowed }
            val tokens = filteredText.tokenize(tokenDividers)
            val mapifiedTokens = mapify(tokens)

            return World()
        }

        private fun String.tokenize(dividers: List<Char>): List<String> {
            val tokens = mutableListOf<String>()
            val currentString = mutableListOf<Char>()
            for (char: Char in this) {
                if (char in dividers) {
                    if (currentString.size > 0) tokens.add(currentString.joinToString(""))
                    currentString.clear()
                    tokens.add(char.toString())
                } else {
                    currentString.add(char)
                }
            }
            return tokens.toList()
        }

        private fun mapify(tokens: List<String>): ObjectHolder {
            val tokenIterator = mutableListOf<String>().apply {
                addAll(tokens)
                add("}")
            }.iterator()

            return handleObject("base", "base", tokenIterator)
        }

        private fun handlePrimitive(name: String, type: String, tokenIterator: Iterator<String>): PrimitiveHolder {
            val primitiveValue = mutableListOf<String>()

            while (tokenIterator.hasNext()) {
                val token = tokenIterator.next()
                if (token == ")") {
                    return PrimitiveHolder(name, primitiveValue.joinToString(""))
                } else {
                    primitiveValue.add(token)
                }
            }

            throw MalformedItemException("primitive $name", "has no closing ')'")
        }

        private fun handleList(name: String, type: String, tokenIterator: Iterator<String>): ListHolder {
            val listHolder = ListHolder(type)
            var currentListObject: ObjectHolder? = null
            var currentListElement: String? = null

            while (tokenIterator.hasNext()) {
                when (val token = tokenIterator.next()) {
                    "=" -> throw UnexpectedCharacterException('=', "list $name", "cannot use assignment in list")
                    "}" -> throw UnexpectedCharacterException('}', "list $name", "cannot close undeclared object in list")
                    "[" -> throw UnexpectedCharacterException('[', "list $name", "cannot declare list in list")
                    "(" -> throw UnexpectedCharacterException('(', "list $name", "cannot declare primitive in list")
                    ")" -> throw UnexpectedCharacterException(')', "list $name", "cannot declare primitive in list")

                    "{" -> {
                        if (currentListObject != null) throw UnexpectedCharacterException('{', "list $name", "list elements must be separated with commas")
                        currentListObject = handleObject("list $name", type, tokenIterator)
                    }

                    "," -> {
                        if ((currentListElement == null) and (currentListObject == null)) throw UnexpectedCharacterException(',', "list $name", "cannot have empty element in list")
                        if (currentListElement != null) listHolder.listOfStringPrimitives.add(currentListElement)
                        if (currentListObject != null) listHolder.listOfObjects.add(currentListObject)
                        currentListElement = null
                        currentListObject = null
                    }

                    "]" -> {
                        if (currentListElement != null) listHolder.listOfStringPrimitives.add(currentListElement)
                        if (currentListObject != null) listHolder.listOfObjects.add(currentListObject)
                        if ((listHolder.listOfObjects.size > 0) and (listHolder.listOfStringPrimitives.size > 0)) throw MalformedItemException("list $name", "cannot have both primitives and objects")
                        return listHolder
                    }

                    else -> {
                        if (currentListElement != null) throw UnexpectedCharacterException('{', "list $name", "list elements must be separated with commas")
                        currentListElement = token
                    }
                }
            }
            throw MalformedItemException("list $name", "has no closing ']'")
        }

        private fun handleObject(name: String, type: String, tokenIterator: Iterator<String>): ObjectHolder {
            val objectHolder = ObjectHolder(type)
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
                          objectHolder.map[variableName] = handleList(variableName, variableType, tokenIterator)
                          assignmentCompleted = true
                      }

                      "(" -> {
                          if (variableName == null) throw UnexpectedCharacterException('(', "object $name", "cannot declare primitive without a variable name")
                          if (beforeAssignment) throw UnexpectedCharacterException('(', "object $name", "cannot declare primitive without an assignment")
                          if (variableType == null) throw UnexpectedCharacterException('(', "object $name", "cannot declare primitive $variableName without a type")
                          objectHolder.map[variableName] = handlePrimitive(variableName, variableType, tokenIterator)
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

        private fun castToWorld(objectHolder: ObjectHolder): World {
            if (objectHolder.map.containsKey("world").not()) throw MalformedItemException("file", "has no variable named world")
            if (objectHolder.map.size > 1) println("Warning: ignoring base variables that aren't 'world'")

            val worldObjectHolder = objectHolder.map["world"]
            if (worldObjectHolder !is ObjectHolder) throw MalformedItemException("file", "has no variable named world of type World")
            return (worldObjectHolder.cast() as? World) ?: throw MalformedItemException("file", "has no world variable of type World")
        }

        private fun castToList(listHolder: ListHolder): List<Any?> {
            return emptyList()
        }

        private fun ObjectHolder.cast(): Any {
            val mapOfAny = mutableMapOf<String, Any>()
            when (type) {

                "World" -> return World (
                    (map["pops"] as? ListHolder)?.cast() ?: throw MalformedItemException("World.pops", "should be a list of Pop objects"),
                    (map["attributes"] as? ListHolder)?.cast() ?: throw MalformedItemException("World.attributes", "should be a list of Attribute objects")
                )

                "Pop" -> return Pop(
                    (map["id"] as? PrimitiveHolder)?.cast()
                )
            }
        }

        private inline fun <reified T> ListHolder.cast(): List<T> {
            val listOfAny = mutableListOf<T>()

            for (item in listOfStringPrimitives) {
                listOfAny.add((castPrimitive(type, item) as? T) ?: throw FileCastException(item, type))
            }
            if (listOfAny.size > 0) return listOfAny.toList()

            for (item in listOfObjects) listOfAny.add((item.cast() as? T) ?: throw FileCastException(item.type, type))
            return listOfAny.toList()
        }

        private inline fun <reified T> PrimitiveHolder.cast(): T = castPrimitive(type, primitive)

        private inline fun <reified T> castPrimitive(type: String, primitive: String): T {
            var castPrimitive: Any? = null
            when (type) {
                "Int" -> castPrimitive = primitive.toIntOrNull() ?: throw FileCastException(primitive, type)
                "Float" ->
            }
            val castPrimitive: Any = primitivesToCastingFunctions[type]?.invoke(primitive) ?: throw NoSuchTypeException(type)
            if (castPrimitive )
        }

        data class ObjectHolder (val type: String) {
            val map = mutableMapOf<String, Any>()
        }
        data class ListHolder (val type: String) {
            val listOfObjects = mutableListOf<ObjectHolder>()
            val listOfStringPrimitives = mutableListOf<String>()
        }
        data class PrimitiveHolder (val type: String, val primitive: String)

        class UnexpectedCharacterException(character: Char, location: String, context: String) :
            Exception("Unexpected character $character in $location: $context")

        class MalformedItemException(item: String, context: String) :
            Exception("Malformed $item $context")

        class FileCastException(item: String, type: String) :
            Exception("Illegal cast: $item cannot be cast to $type")

        class NoSuchTypeException(type: String) :
            Exception("No such type as $type")
    }
}