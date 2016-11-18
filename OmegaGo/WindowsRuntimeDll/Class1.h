#pragma once

namespace WindowsRuntimeDll
{
    public ref class Class1 sealed
    {
    public:
        static int Return4() {
            int local = 4;
            int nextLocal = 6;
            int sum = nextLocal + local;
            int* pointer;
            int* pole = new int[4];
            pole[0] = local;
            pole[1] = nextLocal;
            pole[2] = sum;
            pointer = &pole[2];
            pointer -= 2;
            return *pointer;
        };
        Class1();
    };
}
