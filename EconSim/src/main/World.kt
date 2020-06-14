package main

import org.w3c.dom.Attr
import pop.Attribute
import pop.Pop
import java.lang.Exception

class World (
    private val pops: List<Pop>,
    private val attributes: List<Attribute>
) {

    override fun toString(): String {
        return "World{" +
                "pops=[${pops.joinToString(",")}]," +
                "attributes=[${attributes.joinToString(",")}]" +
                "}"
    }

}