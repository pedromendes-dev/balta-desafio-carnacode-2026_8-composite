#:property PublishAot=false

// DESAFIO: Sistema de Menus Hierárquicos
// PROBLEMA: Um sistema de gestão de conteúdo precisa construir menus com itens simples e submenus aninhados
// O código atual trata itens individuais e grupos de forma diferente, complicando operações recursivas

using System;
using System.Collections.Generic;

namespace DesignPatternChallenge
{
    public abstract class MenuComponent
    {
        public string Title { get; set; }
        public string Icon { get; set; }
        public bool IsActive { get; set; }

        protected MenuComponent(string title, string icon = "")
        {
            Title = title;
            Icon = icon;
            IsActive = true;
        }

        public virtual void Add(MenuComponent component)
        {
            throw new NotSupportedException("Operação não suportada para este componente.");
        }

        public virtual void Remove(MenuComponent component)
        {
            throw new NotSupportedException("Operação não suportada para este componente.");
        }

        public abstract void Render(int indent = 0);
        public abstract int CountItems();
        public abstract void DisableAll();
        public abstract MenuItem? FindByUrl(string url);
    }

    public class MenuItem : MenuComponent
    {
        public string Url { get; set; }

        public MenuItem(string title, string url, string icon = "") : base(title, icon)
        {
            Url = url;
        }

        public override void Render(int indent = 0)
        {
            var indentation = new string(' ', indent * 2);
            var activeStatus = IsActive ? "✓" : "✗";
            Console.WriteLine($"{indentation}[{activeStatus}] {Icon} {Title} → {Url}");
        }

        public override int CountItems() => 1;

        public override void DisableAll() => IsActive = false;

        public override MenuItem? FindByUrl(string url) => Url == url ? this : null;
    }

    public class MenuGroup : MenuComponent
    {
        private readonly List<MenuComponent> _children = new();

        public MenuGroup(string title, string icon = "") : base(title, icon)
        {
        }

        public override void Add(MenuComponent component)
        {
            _children.Add(component);
        }

        public override void Remove(MenuComponent component)
        {
            _children.Remove(component);
        }

        public override void Render(int indent = 0)
        {
            var indentation = new string(' ', indent * 2);
            var activeStatus = IsActive ? "✓" : "✗";
            Console.WriteLine($"{indentation}[{activeStatus}] {Icon} {Title} ▼");

            foreach (var child in _children)
            {
                child.Render(indent + 1);
            }
        }

        public override int CountItems()
        {
            var count = 0;
            foreach (var child in _children)
            {
                count += child.CountItems();
            }
            return count;
        }

        public override void DisableAll()
        {
            IsActive = false;

            foreach (var child in _children)
            {
                child.DisableAll();
            }
        }

        public override MenuItem? FindByUrl(string url)
        {
            foreach (var child in _children)
            {
                var found = child.FindByUrl(url);
                if (found != null)
                    return found;
            }

            return null;
        }
    }

    public class MenuManager
    {
        private readonly MenuGroup _root;

        public MenuManager()
        {
            _root = new MenuGroup("Menu Principal", "🧭");
        }

        public void Add(MenuComponent component)
        {
            _root.Add(component);
        }

        public void RenderMenu()
        {
            Console.WriteLine("=== Menu Principal ===\n");
            _root.Render();
        }

        public int GetTotalItems() => _root.CountItems();

        public MenuItem? FindItemByUrl(string url)
        {
            return _root.FindByUrl(url);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Sistema de Menus CMS ===\n");

            var manager = new MenuManager();

            manager.Add(new MenuItem("Home", "/", "🏠"));

            var productsMenu = new MenuGroup("Produtos", "📦");
            productsMenu.Add(new MenuItem("Todos", "/produtos"));
            productsMenu.Add(new MenuItem("Categorias", "/categorias"));
            productsMenu.Add(new MenuItem("Ofertas", "/ofertas"));

            var clothingMenu = new MenuGroup("Roupas", "👕");
            clothingMenu.Add(new MenuItem("Camisetas", "/roupas/camisetas"));
            clothingMenu.Add(new MenuItem("Calças", "/roupas/calcas"));
            productsMenu.Add(clothingMenu);

            manager.Add(productsMenu);

            var adminMenu = new MenuGroup("Administração", "⚙️");
            adminMenu.Add(new MenuItem("Usuários", "/admin/usuarios"));
            adminMenu.Add(new MenuItem("Configurações", "/admin/config"));
            manager.Add(adminMenu);

            manager.RenderMenu();

            Console.WriteLine($"\nTotal de itens no menu: {manager.GetTotalItems()}");

            var item = manager.FindItemByUrl("/roupas/camisetas");
            if (item != null)
            {
                Console.WriteLine($"\n✓ Item encontrado: {item.Title}");
            }

            Console.WriteLine("\n=== COMPOSITE APLICADO ===");
            Console.WriteLine("✓ Item e grupo tratados pela mesma abstração");
            Console.WriteLine("✓ Operações recursivas centralizadas na árvore");
            Console.WriteLine("✓ Cliente não precisa diferenciar folha e composição");
            Console.WriteLine("✓ Estrutura hierárquica flexível e extensível");
        }
    }
}
