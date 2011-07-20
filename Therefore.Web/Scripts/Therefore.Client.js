/// <reference path="jquery-1.4.4-vsdoc.js" />

var Therefore = {
    rootUrl: "/",
    nodeElementName: "span",
    variableNodeClass: "variable-node",
    binaryOperatorNodeClass: "binary-operator-node",
    unaryNodeClass: "unary-operator-node",
    parenthesisNodeClass: "parenthesis-node"
};

Therefore.parse = function (statement, callback) {
    $.get(this.rootUrl + "Game/Parse", { statement: statement }, callback, "json");
}

Therefore.renderTree = function (parseTree) {
    var me = this;

    var renderNode, renderVariableNode, renderBinaryOpNode, renderUnaryOpNode, renderParenNode;

    var createNode = function (className, args) {
        var node = document.createElement(me.nodeElementName);
        node.setAttribute("class", className);

        for (var i = 1; i < arguments.length; i++) {
            var item = arguments[i];
            if (typeof item == "string") {
                node.appendChild(document.createTextNode(item));
            } else {
                node.appendChild(item);
            }
        }

        return node;
    };

    renderNode = function (node) {
        switch (node.NodeType) {
            case "VariableNode": return renderVariableNode(node);
            case "UnaryOperatorNode": return renderUnaryOpNode(node);
            case "BinaryOperatorNode": return renderBinaryOpNode(node);
            case "ParenthesisNode": return renderParenNode(node);
        }
    };

    renderVariableNode = function (node) {
        return createNode(me.variableNodeClass,
            node.Variable.Value);
    };

    renderUnaryOpNode = function (node) {
        return createNode(me.binaryOperatorNodeClass,
            node.Operator.Value,
            renderNode(node.Operand));
    };

    renderBinaryOpNode = function (node) {
        return createNode(me.binaryOperatorNodeClass,
            renderNode(node.Left),
            node.Operator.Value,
            renderNode(node.Right));
    };

    renderParenNode = function (node) {
        return createNode(me.parenthesisNodeClass,
            node.LeftParenthesis.Value,
            renderNode(node.Contained),
            node.RightParenthesis.Value);
    };

    return renderNode(parseTree.RootNode);
}
