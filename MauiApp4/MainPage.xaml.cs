
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Maui.Markup;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Essentials;

namespace MauiApp
{
    public partial class MainPage : ContentPage
    {

        public class MainPageViewModel : ObservableObject
        {

            private const string BaseAddress = "https://jsonplaceholder.typicode.com/posts";


            private readonly HttpClient _client;


            private List<PostRecord> _posts;

            private PostRecord _selectedPost;


            public MainPageViewModel()
            {

                _client = new HttpClient();


                _posts = new List<PostRecord>();


                LoadPostsCommand = new AsyncRelayCommand(LoadPostsAsync);
                AddPostCommand = new AsyncRelayCommand(AddPostAsync);
                EditPostCommand = new AsyncRelayCommand(EditPostAsync);
                DeletePostCommand = new AsyncRelayCommand(DeletePostAsync);
            }


            public List<PostRecord> Posts
            {
                get => _posts;
                set => SetProperty(ref _posts, value);
            }


            public PostRecord SelectedPost
            {
                get => _selectedPost;
                set => SetProperty(ref _selectedPost, value);
            }


            public IAsyncRelayCommand LoadPostsCommand { get; }


            public IAsyncRelayCommand AddPostCommand { get; }
            public IAsyncRelayCommand EditPostCommand { get; }


            public IAsyncRelayCommand DeletePostCommand { get; }


            private async Task LoadPostsAsync()
            {
                try
                {

                    var response = await _client.GetAsync(BaseAddress);


                    if (response.IsSuccessStatusCode)
                    {

                        var json = await response.Content.ReadAsStringAsync();


                        var posts = JsonSerializer.Deserialize<List<PostRecord>>(json);


                        Posts = posts;
                    }
                    else
                    {

                        await DisplayAlert("Error", "Failed to load posts from the API.", "OK");
                    }
                }
                catch (Exception ex)
                {

                    await DisplayAlert("Exception", ex.Message, "OK");
                }
            }


            private async Task AddPostAsync()
            {
                try
                {

                    var post = new PostRecord
                    {
                        UserId = "1",
                        Title = "New post",
                        Body = "This is a new post."
                    };


                    var json = JsonSerializer.Serialize(post);


                    var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                    var response = await _client.PostAsync(BaseAddress, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();

                        var newPost = JsonSerializer.Deserialize<PostRecord>(result);

                        Posts.Add(newPost);

                        Posts = new List<PostRecord>(Posts);
                    }
                    else
                    {
                        await DisplayAlert("Error", "Failed to add a new post to the API.", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Exception", ex.Message, "OK");
                }
            }

            private async Task EditPostAsync()
            {
                try
                {

                    if (SelectedPost != null)
                    {

                        var id = SelectedPost.Id;

                        var post = new PostRecord
                        {
                            Id = id,
                            UserId = SelectedPost.UserId,
                            Title = SelectedPost.Title + " (edited)",
                            Body = SelectedPost.Body + " (edited)"
                        };

                        var json = JsonSerializer.Serialize(post);

                        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                        var response = await _client.PutAsync($"{BaseAddress}/{id}", content);

                        if (response.IsSuccessStatusCode)
                        {
                            var result = await response.Content.ReadAsStringAsync();

                            var updatedPost = JsonSerializer.Deserialize<PostRecord>(result);

                            var index = Posts.FindIndex(p => p.Id == id);

                            Posts[index] = updatedPost;

                            Posts = new List<PostRecord>(Posts);
                        }
                        else
                        {
                            await DisplayAlert("Error", "Failed to edit the post in the API.", "OK");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Warning", "Please select a post to edit.", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Exception", ex.Message, "OK");
                }
            }

            private async Task DeletePostAsync()
            {
                try
                {
                    if (SelectedPost != null)
                    {
                        var id = SelectedPost.Id;

                        var confirmed = await DisplayAlert("Confirm", "Are you sure you want to delete this post?", "Yes", "No");

                        if (confirmed)
                        {
                            var response = await _client.DeleteAsync($"{BaseAddress}/{id}");

                            if (response.IsSuccessStatusCode)
                            {
                                Posts.Remove(SelectedPost);

                                Posts = new List<PostRecord>(Posts);
                            }
                            else
                            {
                                await DisplayAlert("Error", "Failed to delete the post from the API.", "OK");
                            }
                        }
                    }
                    else
                    {
                        await DisplayAlert("Warning", "Please select a post to delete.", "OK");
                    }
                }
                catch (Exception ex)
                {

                    await DisplayAlert("Exception", ex.Message, "OK");
                }
            }
        }
    }
}


          