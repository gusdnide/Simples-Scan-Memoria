#include <Windows.h>
#include <iostream>
using namespace std;
int Ler;
int Vida = 100;
int main()
{
	cout << "Eae bora, testar o App\n";
	cout << "by gusdnide\n\n\n";
	cout << "Comandos: 1 = Atacar, 2 = Resetar, 3 = Mostrar Vida, 4 = Sair \n";
	cout << "Pressione Enter, para Mandar o Comando ou Passar...\n";
	cout << "\n";
	cout << "Jogo Iniciado";
	cout << "\n\n\n";
	cout << "Sua Vida: " << Vida <<"\n";
	cout << "\n";
	while(Ler != 4)
	{
		cin >> Ler;
		cout << "\n";
		if(Ler == 1)
		{
			if(Vida > 0)
			{
				Vida -= 5;
				cout << "Atacado -5 \n";
			}
			else
			{ 
				Vida = 100;
				cout << "Vida Resetada\n";
			}
		}
		else if(Ler == 2)
		{
			Vida = 100;
			cout << "Vida Resetada\n";
		}
		else if(Ler == 3)
		{
			cout << "Sua Vida: " << Vida  <<"\n";
		}
		else if(Ler == 4)
		{
			cout << "Saindo\n";
			exit(0);
		}
		else
		{
			cout << "...";
		}
	}
}