using System;

public class AvlNode
{
    public int Key;
    public int Height;
    public AvlNode Left;
    public AvlNode Right;

    public AvlNode(int key)
    {
        Key = key;
        Height = 1;
    }
}

public class AvlTree
{
    private AvlNode root;

    private int Height(AvlNode node)
    {
        return node == null ? 0 : node.Height;
    }

    private int GetBalance(AvlNode node)
    {
        return node == null ? 0 : Height(node.Left) - Height(node.Right);
    }

    private void UpdateHeight(AvlNode node)
    {
        node.Height = 1 + Math.Max(Height(node.Left), Height(node.Right));
    }

    private AvlNode RotateRight(AvlNode y)
    {
        AvlNode x = y.Left;
        AvlNode temp = x.Right;

        x.Right = y;
        y.Left = temp;

        UpdateHeight(y);
        UpdateHeight(x);

        return x;
    }

    private AvlNode RotateLeft(AvlNode x)
    {
        AvlNode y = x.Right;
        AvlNode temp = y.Left;

        y.Left = x;
        x.Right = temp;

        UpdateHeight(x);
        UpdateHeight(y);

        return y;
    }

    private AvlNode Insert(AvlNode node, int key)
    {
        if (node == null)
            return new AvlNode(key);

        if (key < node.Key)
            node.Left = Insert(node.Left, key);
        else if (key > node.Key)
            node.Right = Insert(node.Right, key);
        else
            return node;

        UpdateHeight(node);

        int balance = GetBalance(node);

        // Left Left
        if (balance > 1 && key < node.Left.Key)
            return RotateRight(node);

        // Right Right
        if (balance < -1 && key > node.Right.Key)
            return RotateLeft(node);

        // Left Right
        if (balance > 1 && key > node.Left.Key)
        {
            node.Left = RotateLeft(node.Left);
            return RotateRight(node);
        }

        // Right Left
        if (balance < -1 && key < node.Right.Key)
        {
            node.Right = RotateRight(node.Right);
            return RotateLeft(node);
        }

        return node;
    }

    public void Insert(int key)
    {
        root = Insert(root, key);
    }

    public bool Search(int key)
    {
        AvlNode current = root;

        while (current != null)
        {
            if (key == current.Key)
                return true;

            current = key < current.Key
                ? current.Left
                : current.Right;
        }

        return false;
    }

    public void InOrder()
    {
        InOrder(root);
        Console.WriteLine();
    }

    private void InOrder(AvlNode node)
    {
        if (node == null)
            return;

        InOrder(node.Left);
        Console.Write(node.Key + " ");
        InOrder(node.Right);
    }
}
