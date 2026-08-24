using System;

public static class OptimizedSorter
{
    public static void MergeSort(int[] array)
    {
        if (array == null || array.Length <= 1)
            return;

        int[] temp = new int[array.Length];

        MergeSort(array, temp, 0, array.Length - 1);
    }

    private static void MergeSort(
        int[] array,
        int[] temp,
        int left,
        int right)
    {
        if (left >= right)
            return;

        int middle = left + (right - left) / 2;

        MergeSort(array, temp, left, middle);
        MergeSort(array, temp, middle + 1, right);

        Merge(array, temp, left, middle, right);
    }

    private static void Merge(
        int[] array,
        int[] temp,
        int left,
        int middle,
        int right)
    {
        int i = left;
        int j = middle + 1;
        int k = left;

        while (i <= middle && j <= right)
        {
            if (array[i] <= array[j])
                temp[k++] = array[i++];
            else
                temp[k++] = array[j++];
        }

        while (i <= middle)
            temp[k++] = array[i++];

        while (j <= right)
            temp[k++] = array[j++];

        for (int index = left; index <= right; index++)
            array[index] = temp[index];
    }
}