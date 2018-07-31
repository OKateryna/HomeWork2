using Homework2.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Homework2.Services
{
    public class EntitiesService
    {
        private const string EntityLink = "https://5b128555d50a5c0014ef1204.mockapi.io/";
        private static IEnumerable<Comment> Comments;
        private static IEnumerable<Post> Posts;
        private static IEnumerable<Todo> Todos;
        private static IEnumerable<Address> Addresses;
        private static IEnumerable<User> CompleteUsers;

        public IEnumerable<Comment> AllComments
        {
            get
            {
                return Comments;
            }
        }

        public IEnumerable<Post> AllPosts
        {
            get
            {
                return Posts;
            }
        }

        public IEnumerable<Todo> AllTodos
        {
            get
            {
                return Todos;
            }
        }

        public IEnumerable<Address> AllAddresses
        {
            get
            {
                return Addresses;
            }
        }

        public IEnumerable<User> AllUsers
        {
            get
            {
                return CompleteUsers;
            }
        }
        
        static EntitiesService()
        {
            
            var client = new WebClient();
            string usersJson = client.DownloadString(EntityLink + "users");
            var users = JsonConvert.DeserializeObject<IEnumerable<User>>(usersJson);

            string postsJson = client.DownloadString(EntityLink + "posts");
            Posts = JsonConvert.DeserializeObject<IEnumerable<Post>>(postsJson);

            string todosJson = client.DownloadString(EntityLink + "todos");
            Todos = JsonConvert.DeserializeObject<IEnumerable<Todo>>(todosJson);

            string addressesJson = client.DownloadString(EntityLink + "address");
            Addresses = JsonConvert.DeserializeObject<IEnumerable<Address>>(addressesJson);

            string commentsJson = client.DownloadString(EntityLink + "comments");
            Comments = JsonConvert.DeserializeObject<IEnumerable<Comment>>(commentsJson);

            CompleteUsers = from user in users
                            join post in
                            (from post in Posts
                             join comment in Comments on post.Id equals comment.PostId into commentGroup
                             select new Post
                             {
                                 Id = post.Id,
                                 Title = post.Title,
                                 Body = post.Body,
                                 CreatedAt = post.CreatedAt,
                                 Likes = post.Likes,
                                 UserId = post.UserId,
                                 Comments = commentGroup
                             })
                             on user.Id equals post.UserId into postGroup
                            join todo in Todos on user.Id equals todo.UserId into todoGroup
                            join adress in Addresses on user.Id equals adress.UserId
                            select new User
                            {
                                Name = user.Name,
                                Id = user.Id,
                                Avatar = user.Avatar,
                                CreatedAt = user.CreatedAt,
                                Email = user.Email,
                                Posts = postGroup,
                                Todos = todoGroup,
                                Adress = adress
                            };
        }

        /// <summary>
        /// Task6: Получить следующую структуру(передать Id поста в параметры)
        /// Пост
        /// Самый длинный коммент поста
        /// Самый залайканный коммент поста
        /// Количество комментов под постом где или 0 лайков или длина текста< 80
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        public Tuple<Post, Comment, Comment, int> GetPostInfo(int postId)
        {
            var pos = CompleteUsers.SelectMany(x => x.Posts).Where(i => i.Id == postId).Select(post => (new
            Tuple<Post, Comment, Comment, int>(
                post,
                post.Comments.OrderByDescending(a => a.Body.Length).FirstOrDefault(),
                post.Comments.OrderByDescending(s => s.Likes).FirstOrDefault(),
                post.Comments.Where(o => o.Likes == 0 || o.Body.Length < 80).Count()
                ))).FirstOrDefault();
            return pos;
        }

        /// <summary>
        /// Task5: 
        /// Получить следующую структуру(передать Id пользователя в параметры)
        /// User
        /// Последний пост пользователя(по дате)
        /// Количество комментов под последним постом
        /// Количество невыполненных тасков для пользователя
        /// Самый популярный пост пользователя(там где больше всего комментов с длиной текста больше 80 символов)
        /// Самый популярный пост пользователя(там где больше всего лайков)
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Tuple<User, Post, int, int, Post, Post> GetFilteredInfo(int userId)
        {
            var curentUser = CompleteUsers.Where(x => x.Id == userId).Select(user => new Tuple<User, Post, int, int, Post, Post>
            (   user,
                user.Posts.OrderBy(post => post.CreatedAt).FirstOrDefault(),
                user.Posts.OrderByDescending(post => post.CreatedAt).FirstOrDefault()?.Comments.Count() ?? 0,
                user.Todos.Count(todo => !todo.IsComplete),
                user.Posts.OrderByDescending(post => post.Comments.Count(comment => comment.Body.Length > 80)).FirstOrDefault(),
                user.Posts.OrderByDescending(post => post.Likes).FirstOrDefault()
                )).FirstOrDefault();
            return curentUser;
        }

        /// <summary>
        /// Task4: Получить список пользователей по алфавиту(по возрастанию) с отсортированными todo items по длине name(по убыванию)
        /// </summary>
        /// <returns></returns>
        public IEnumerable<User> SortUsersByNameAndTodoNameLenght()
        {
            var userList = CompleteUsers.OrderBy(a => a.Name).Select(user => new User
            {
                Name = user.Name,
                Id = user.Id,
                Avatar = user.Avatar,
                CreatedAt = user.CreatedAt,
                Email = user.Email,
                Posts = user.Posts,
                Todos = user.Todos.OrderByDescending(a => a.Name.Length),
                Adress = user.Adress,
            });
            return userList;
        }

        /// <summary>
        /// Task3: Получить список(id, name) из списка todos которые выполнены для конкретного пользователя(по айди)
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IEnumerable<Tuple<int, string>> GetIncompleteTodosForUser(int userId)
        {
            var listidname = CompleteUsers.FirstOrDefault(x => x.Id == userId).Todos.Where(i => i.IsComplete).Select(todo => new Tuple<int, string>( todo.Id, todo.Name));
            return listidname;
        }

        /// <summary>
        /// Task2: Получить список комментов под постами конкретного пользователя(по айди), где body коммента<50 символов(список из комментов)
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IEnumerable<Comment> GetShortCommentsForUser(int userId)
        {
            var listcom = CompleteUsers.FirstOrDefault(x => x.Id == userId).Posts.SelectMany(post => post.Comments).Where(c => c.Body.Length < 50);
            return listcom;
        }

        /// <summary>
        /// Task1: Получить количество комментов под постами конкретного пользователя(по айди)(список из пост - количество)
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// 
        public IEnumerable<Tuple<Post, int>> GetCommentCountForUser(int userId)
        {
            var result = CompleteUsers.FirstOrDefault(x => x.Id == userId)?.Posts
                .Select(x => new Tuple<Post, int>(x, x.Comments.Count()));
            return result;
        }
    }
}
